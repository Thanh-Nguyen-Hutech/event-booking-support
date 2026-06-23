using EventBooking.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Kiểm tra giá trị hợp lệ cho phân trang
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa 100 item/trang để tránh quá tải

            // Bắt đầu truy vấn (Dùng AsNoTracking để tăng tốc độ cho GET)
            var query = _context.ServicePackages.AsNoTracking();

            // 1. Áp dụng Search (Nếu có keyword)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.PackageName.Contains(keyword) ||
                    p.Description.Contains(keyword));
            }

            // 2. Tính tổng số lượng bản ghi (để trả về Metadata cho Frontend)
            var totalCount = await query.CountAsync();

            // 3. Áp dụng Phân trang (Skip: bỏ qua, Take: lấy số lượng)
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. Trả về kết quả kèm thông tin phân trang
            return Ok(new
            {
                Metadata = new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                },
                Items = items
            });
        }
    }
}