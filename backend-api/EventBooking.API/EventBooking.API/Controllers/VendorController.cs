using EventBooking.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetVendorAvailability(Guid id)
        {
            // 1. Kiểm tra Vendor có tồn tại không
            var vendorExists = await _context.VendorProfiles.AnyAsync(v => v.VendorId == id);
            if (!vendorExists)
            {
                return NotFound(new { Message = "Không tìm thấy Vendor." });
            }

            // 2. Lấy danh sách ngày bận
            var busyDates = await _context.Bookings
                .Where(b => b.ServicePackage.VendorId == id) 
                .Where(b => b.EventDate >= DateTime.UtcNow) 
                .Select(b => b.EventDate.Date)             
                .Distinct()                                 
                .OrderBy(d => d)                             
                .ToListAsync();

            return Ok(busyDates);
        }
    }
}