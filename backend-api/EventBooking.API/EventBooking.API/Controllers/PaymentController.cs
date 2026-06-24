using EventBooking.API.Data;
using EventBooking.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly VnPayService _vnPayService;

        public PaymentController(ApplicationDbContext context, VnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        [HttpPost("create-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] Guid bookingId)
        {
            // 1. Tìm đơn hàng
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound(new { Message = "Không tìm thấy đơn đặt lịch." });
            }

            // 2. Kiểm tra xem đơn có nằm trong thời gian 15 phút không
            var expireTime = DateTime.Now.AddMinutes(-15);
            if (booking.Status != 0 || booking.CreatedAt < expireTime)
            {
                return BadRequest(new { Message = "Đơn này đã thanh toán hoặc đã quá hạn 15 phút giữ chỗ." });
            }

            // 3. Tạo URL Thanh Toán
            // Truyền DepositAmount (Tiền cọc) để khách thanh toán
            var url = _vnPayService.CreatePaymentUrl(
                HttpContext,
                booking.BookingId,
                booking.DepositAmount,
                $"Thanh toan tien coc cho Booking {booking.BookingId}"
            );

            // 4. Trả về cho Client
            return Ok(new { PaymentUrl = url });
        }

        // API này hứng dữ liệu do VNPAY đẩy về
        // Đường dẫn sẽ là: GET https://localhost:7201/payment-result
        [HttpGet("/payment-result")]
        public async Task<IActionResult> PaymentCallback()
        {
            var queryDictionary = Request.Query;

            // 1. Kiểm tra chữ ký bảo mật xem có bị hack không
            if (!_vnPayService.ValidateSignature(queryDictionary))
            {
                return BadRequest(new { Message = "Chữ ký bảo mật không hợp lệ! Phát hiện nghi vấn gian lận." });
            }

            // 2. Đọc mã phản hồi và ID đơn hàng
            string vnp_ResponseCode = queryDictionary["vnp_ResponseCode"]!;
            string bookingIdStr = queryDictionary["vnp_TxnRef"]!; // VNPAY trả ID vào trường này

            if (Guid.TryParse(bookingIdStr, out Guid bookingId))
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                if (booking != null)
                {
                    // VNPAY quy định mã "00" là Thanh Toán Thành Công
                    if (vnp_ResponseCode == "00")
                    {
                        // CHỐT ĐƠN: Đổi trạng thái sang 1 (Đã cọc)
                        booking.Status = 1;
                        await _context.SaveChangesAsync();

                        // Trong thực tế, chỗ này bạn có thể Redirect về một trang HTML thông báo thành công của Frontend
                        return Ok(new { Message = "Thanh toán thành công! Đơn của bạn đã được chốt lịch vững chắc." });
                    }
                    else
                    {
                        // Các mã khác (như 24: Khách tự hủy giao dịch)
                        return BadRequest(new { Message = "Giao dịch thất bại hoặc đã bị hủy bởi người dùng.", ErrorCode = vnp_ResponseCode });
                    }
                }
            }

            return NotFound(new { Message = "Không tìm thấy đơn hàng tương ứng với giao dịch này." });
        }
    }
}