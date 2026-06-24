using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EventBooking.API.Services
{
    public class VnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HttpContext context, Guid bookingId, decimal amount, string orderInfo)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _configuration["VnPay:TmnCode"]! },
                { "vnp_Amount", ((long)(amount * 100)).ToString() }, // VNPAY yêu cầu nhân 100
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]! },
                { "vnp_TxnRef", bookingId.ToString() } // Mã tham chiếu là ID của Booking
            };

            // Build query string
            var queryString = new StringBuilder();
            foreach (var kv in vnpayData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    queryString.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            var signData = queryString.ToString().TrimEnd('&');

            // Tạo chữ ký bảo mật (Secure Hash)
            var vnp_SecureHash = HmacSHA512(_configuration["VnPay:HashSecret"]!, signData);

            // Link cuối cùng để gửi về FE
            return $"{_configuration["VnPay:BaseUrl"]}?{signData}&vnp_SecureHash={vnp_SecureHash}";
        }

        public bool ValidateSignature(IQueryCollection queryData)
        {
            var vnpayData = new SortedDictionary<string, string>();
            string? vnp_SecureHash = queryData["vnp_SecureHash"];

            // Lọc lấy các tham số bắt đầu bằng "vnp_" (loại bỏ chữ ký cũ)
            foreach (var kv in queryData)
            {
                if (!string.IsNullOrEmpty(kv.Value) && kv.Key.StartsWith("vnp_") &&
                    kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                {
                    vnpayData.Add(kv.Key, kv.Value.ToString());
                }
            }

            // Xếp lại chuỗi và băm thử
            var queryString = new StringBuilder();
            foreach (var kv in vnpayData)
            {
                queryString.Append(System.Net.WebUtility.UrlEncode(kv.Key) + "=" + System.Net.WebUtility.UrlEncode(kv.Value) + "&");
            }
            var signData = queryString.ToString().TrimEnd('&');

            // Băm chuỗi thu được với HashSecret
            var checkHash = HmacSHA512(_configuration["VnPay:HashSecret"]!, signData);

            // So sánh chữ ký mình tự tính với chữ ký VNPAY gửi về
            return checkHash.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}