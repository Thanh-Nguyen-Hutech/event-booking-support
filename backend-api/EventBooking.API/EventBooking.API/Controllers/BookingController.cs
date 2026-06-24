using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Lấy thông tin Gói dịch vụ và Vendor
                var package = await _context.ServicePackages
                    .Include(p => p.VendorProfile)
                    .FirstOrDefaultAsync(p => p.PackageId == dto.PackageId);

                if (package == null)
                {
                    return NotFound(new { Message = "Không tìm thấy gói dịch vụ." });
                }

                var vendorId = package.VendorId;

                // 2. KIỂM TRA QUY TẮC: Vendor có đang đóng cửa tạm thời không?
                if (package.VendorProfile != null && package.VendorProfile.IsTemporarilyClosed)
                {
                    return BadRequest(new { Message = "Thợ ảnh đang tạm nghỉ, không thể nhận lịch lúc này." });
                }

                // 3. KIỂM TRA QUY TẮC: Ngày này có nằm trong danh sách Ngày Nghỉ (BlockedDates) không?
                // Lưu ý: Nếu bạn chưa tạo bảng BlockedDates, có thể comment tạm đoạn này lại
                /*
                var isBlocked = await _context.BlockedDates
                    .AnyAsync(b => b.VendorId == vendorId && b.Date.Date == dto.EventDate.Date);

                if (isBlocked)
                {
                    return BadRequest(new { Message = "Thợ ảnh có việc bận không nhận khách vào ngày này." });
                }
                */

                // 4. KIỂM TRA TRÙNG LỊCH (CÓ GIỮ CHỖ 15 PHÚT)
                var expireTime = DateTime.Now.AddMinutes(-15); 

                var isTaken = await _context.Bookings
                    .AnyAsync(b => b.PackageId == dto.PackageId
                                && b.EventDate.Date == dto.EventDate.Date
                                && b.TimeSlot == dto.TimeSlot
                                && (
                                    b.Status == 1 ||
                                    b.Status == 2 || 
                                    (b.Status == 0 && b.CreatedAt > expireTime) 
                                ));

                if (isTaken)
                {
                    return BadRequest(new { Message = "Khung giờ này đang có người khác giữ chỗ để thanh toán. Vui lòng quay lại sau 15 phút." });
                }

                // 5. VƯỢT QUA MỌI KIỂM TRA -> TẠO BOOKING MỚI
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    PackageId = dto.PackageId,
                    EventDate = dto.EventDate,
                    TimeSlot = dto.TimeSlot,
                    TotalAmount = dto.TotalAmount,
                    DepositAmount = dto.DepositAmount,
                    EventLocation = dto.EventLocation,
                    Status = 0, // Pending: Chờ thanh toán
                    CreatedAt = DateTime.Now 
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Có lỗi xảy ra khi tạo booking.", Detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }
    }
}