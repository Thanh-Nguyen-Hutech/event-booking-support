using EventBooking.API.Data;
using EventBooking.API.DTOs;
using EventBooking.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. API: Gửi đánh giá cho một Booking đã hoàn thành
        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO dto)
        {
            // Kiểm tra xem đơn đặt lịch có tồn tại không, kèm theo thông tin gói dịch vụ
            var booking = await _context.Bookings
                .Include(b => b.ServicePackage)
                .FirstOrDefaultAsync(b => b.BookingId == dto.BookingId);

            if (booking == null)
            {
                return NotFound(new { Message = "Không tìm thấy đơn đặt lịch tương ứng." });
            }

            // RÀNG BUỘC 1: Chỉ đơn hàng đã Hoàn tất (Status = 2) mới được đánh giá
            if (booking.Status != 2)
            {
                return BadRequest(new { Message = "Bạn chỉ có thể đánh giá dịch vụ sau khi đơn đặt lịch đã hoàn thành." });
            }

            // RÀNG BUỘC 2: Kiểm tra xem đơn này đã được đánh giá trước đó chưa
            var isAlreadyReviewed = await _context.Reviews.AnyAsync(r => r.BookingId == dto.BookingId);
            if (isAlreadyReviewed)
            {
                return BadRequest(new { Message = "Đơn đặt lịch này đã được đánh giá rồi." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tạo đối tượng Review mới
                var review = new Review
                {
                    ReviewId = Guid.NewGuid(),
                    BookingId = dto.BookingId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // TỰ ĐỘNG CẬP NHẬT AVERAGE RATING CỦA VENDOR
                if (booking.ServicePackage != null)
                {
                    var vendorId = booking.ServicePackage.VendorId;

                    // Lấy tất cả các mức rating của Vendor này thông qua các Booking thành công
                    var allVendorRatings = await _context.Reviews
                        .Include(r => r.Booking)
                        .ThenInclude(b => b!.ServicePackage)
                        .Where(r => r.Booking!.ServicePackage!.VendorId == vendorId)
                        .Select(r => r.Rating)
                        .ToListAsync();

                    // Tính trung bình cộng
                    double averageRating = allVendorRatings.Any() ? allVendorRatings.Average() : 0;

                    // Cập nhật lại vào bảng VendorProfiles
                    var vendorProfile = await _context.VendorProfiles.FindAsync(vendorId);
                    if (vendorProfile != null)
                    {
                        vendorProfile.AverageRating = averageRating;
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return Ok(new { Message = "Cảm ơn bạn đã gửi đánh giá!", Review = review });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Có lỗi xảy ra khi lưu đánh giá.", Detail = ex.Message });
            }
        }

        // 2. API: Lấy toàn bộ danh sách đánh giá của một Vendor cụ thể
        // GET: api/review/vendor/{vendorId}
        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetVendorReviews(Guid vendorId)
        {
            // Kiểm tra Vendor tồn tại
            var vendorExists = await _context.VendorProfiles.AnyAsync(v => v.VendorId == vendorId);
            if (!vendorExists)
            {
                return NotFound(new { Message = "Không tìm thấy nhà cung cấp dịch vụ." });
            }

            // Truy vấn lấy danh sách review, kết hợp lấy tên khách hàng công khai
            var reviews = await _context.Reviews
                .Include(r => r.Booking)
                .ThenInclude(b => b!.Customer) // Để lấy tên khách hàng đã viết review
                .Include(r => r.Booking)
                .ThenInclude(b => b!.ServicePackage)
                .Where(r => r.Booking!.ServicePackage!.VendorId == vendorId)
                .OrderByDescending(r => r.CreatedAt) // Đánh giá mới nhất hiện lên đầu
                .Select(r => new
                {
                    r.ReviewId,
                    r.BookingId,
                    PackageName = r.Booking!.ServicePackage!.PackageName,
                    CustomerName = r.Booking.Customer != null ? r.Booking.Customer.FullName : "Khách hàng ẩn danh",
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}