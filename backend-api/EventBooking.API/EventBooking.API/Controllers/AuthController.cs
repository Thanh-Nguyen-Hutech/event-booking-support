using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}