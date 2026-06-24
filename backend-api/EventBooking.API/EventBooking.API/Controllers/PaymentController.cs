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

        // Đường dẫn sẽ là: GET api/payment/ipn
        [HttpGet("ipn")]
        public async Task<IActionResult> IpnCallback()
        {
            var queryDictionary = Request.Query;

            try
            {
                // 1. Kiểm tra chữ ký bảo mật (Chống request giả mạo)
                if (!_vnPayService.ValidateSignature(queryDictionary))
                {
                    return Ok(new { RspCode = "97", Message = "Invalid signature" });
                }

                string vnp_ResponseCode = queryDictionary["vnp_ResponseCode"]!;
                string bookingIdStr = queryDictionary["vnp_TxnRef"]!;

                // VNPAY nhân số tiền lên 100 lần, nên ta phải chia lại để kiểm tra
                long vnp_Amount = Convert.ToInt64(queryDictionary["vnp_Amount"]) / 100;

                if (Guid.TryParse(bookingIdStr, out Guid bookingId))
                {
                    var booking = await _context.Bookings.FindAsync(bookingId);

                    // 2. Kiểm tra đơn hàng có tồn tại không
                    if (booking == null)
                    {
                        return Ok(new { RspCode = "01", Message = "Order not found" });
                    }

                    // 3. Kiểm tra số tiền thanh toán có khớp với tiền cọc không
                    if (booking.DepositAmount != vnp_Amount)
                    {
                        return Ok(new { RspCode = "04", Message = "Invalid amount" });
                    }

                    // 4. Kiểm tra trạng thái đơn hàng (Chỉ xử lý đơn Pending = 0)
                    if (booking.Status != 0)
                    {
                        return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                    }

                    // 5. CẬP NHẬT DATABASE
                    if (vnp_ResponseCode == "00")
                    {
                        booking.Status = 1; // Giao dịch thành công -> Đã cọc
                    }
                    else
                    {
                        // Có thể tạo thêm trạng thái -1 cho giao dịch thất bại/hủy
                        // Tuy nhiên với cơ chế giữ chỗ 15 phút, bạn có thể cứ giữ nguyên Status 0 để hệ thống tự đào thải
                    }

                    await _context.SaveChangesAsync();

                    // Bắt buộc trả về RspCode = "00" để VNPAY biết bạn đã nhận và xử lý xong
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }

                return Ok(new { RspCode = "99", Message = "Input data required" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi tại đây nếu cần
                return Ok(new { RspCode = "99", Message = "Unknown error" });
            }
        }
    }
}