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
            [FromQuery] int pageSize = 10,
            [FromQuery] decimal? minPrice = null,    // Thêm minPrice
            [FromQuery] decimal? maxPrice = null,    // Thêm maxPrice
            [FromQuery] int? categoryId = null,      // Thêm categoryId
            [FromQuery] string? sortBy = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.ServicePackages.AsNoTracking();

            // 1. Áp dụng Search
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.PackageName.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)));
            }

            // 2. Áp dụng Lọc (Filter)
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

            // 3. Áp dụng Sắp xếp (Sort)
            // Lưu ý: Phải Sắp xếp TRƯỚC khi Skip/Take
            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.PackageName),
                _ => query.OrderByDescending(p => p.PackageName) // Mặc định sắp xếp theo tên
            };

            // 4. Tính toán và Phân trang
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Metadata = new { TotalCount = totalCount, PageSize = pageSize, CurrentPage = page, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) },
                Items = items
            });
        }
    }
}