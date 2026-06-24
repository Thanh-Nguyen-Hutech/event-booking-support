using EventBooking.API.Data;
using EventBooking.API.Models;
using EventBooking.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public PortfolioController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("{portfolioId}/upload-image")]
        public async Task<IActionResult> UploadPortfolioImage(Guid portfolioId, IFormFile file)
        {
            // 1. Validate định dạng và dung lượng file (Bảo mật)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { Message = "Chỉ cho phép định dạng .jpg, .jpeg, .png, .webp" });

            if (file.Length > 20 * 1024 * 1024) // Giới hạn 5MB
                return BadRequest(new { Message = "Dung lượng ảnh không được vượt quá 5MB." });

            // 2. Kiểm tra Portfolio có tồn tại không
            var portfolio = await _context.Portfolios.FindAsync(portfolioId);
            if (portfolio == null)
                return NotFound(new { Message = "Không tìm thấy Portfolio." });

            try
            {
                // 3. Gọi File Service để lưu file vật lý
                string imageUrl = await _fileService.UploadFileAsync(file, "portfolios");

                // 4. Lưu đường dẫn (URL) vào Database
                var portfolioImage = new PortfolioImage
                {
                    ImageId = Guid.NewGuid(),
                    PortfolioId = portfolioId,
                    ImageUrl = imageUrl
                };

                _context.PortfolioImages.Add(portfolioImage);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Tải ảnh lên thành công!",
                    ImageId = portfolioImage.ImageId,
                    ImageUrl = portfolioImage.ImageUrl
                });
            }
            catch (Exception ex)
            {
                // Log lỗi ra hệ thống ở đây
                return StatusCode(500, new { Message = $"Lỗi khi tải file: {ex.Message}" });
            }
        }
    }
}