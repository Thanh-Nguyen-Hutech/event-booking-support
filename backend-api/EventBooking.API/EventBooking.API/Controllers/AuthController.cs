using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            // 1. Kiểm tra xem Email đã tồn tại trong hệ thống chưa
            var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (userExists)
            {
                return BadRequest(new { Message = "Email này đã được đăng ký. Vui lòng sử dụng email khác!" });
            }

            // 2. Kiểm tra RoleId có hợp lệ không (Tùy chọn)
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == request.RoleId);
            if (!roleExists)
            {
                // Nếu DB chưa có Role nào, bạn có thể comment đoạn kiểm tra này lại lúc test ban đầu
                return BadRequest(new { Message = "Vai trò (Role) không tồn tại hợp lệ." });
            }

            // 3. Mã hóa mật khẩu bằng BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Ánh xạ dữ liệu từ DTO sang Model User thực tế
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash, 
                RoleId = request.RoleId,
                IsActive = true
            };

            if (request.RoleId == 2)
            {
                newUser.VendorProfile = new VendorProfile
                {
                    VendorId = Guid.NewGuid(),
                    UserId = newUser.UserId,
                    StudioName = request.FullName + " Studio",
                    AverageRating = 0
                };
            }

            // 5. Lưu vào Database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 6. Trả về kết quả thành công (Không trả về PasswordHash)
            return StatusCode(201, new
            {
                Message = "Đăng ký tài khoản thành công!",
                UserId = newUser.UserId,
                Email = newUser.Email,
                Role = newUser.RoleId
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // 1. Tìm User theo Email (Kèm theo Role để lấy Tên Vai trò)
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. Kiểm tra tài khoản có tồn tại và Mật khẩu có khớp không (Dùng BCrypt Verify)
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Email hoặc mật khẩu không chính xác." });
            }

            // 3. Kiểm tra tài khoản có bị khóa không
            if (!user.IsActive)
            {
                return StatusCode(403, new { Message = "Tài khoản của bạn đã bị khóa." });
            }

            // 4. Tạo JWT Token
            var token = GenerateJwtToken(user);

            // 5. Trả về Token và thông tin cơ bản cho Frontend lưu trữ
            return Ok(new
            {
                Message = "Đăng nhập thành công!",
                Token = token,
                UserInfo = new
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role?.RoleName 
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            // Gói các thông tin (Claims) vào trong Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "") 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["ExpireDays"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}