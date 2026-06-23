namespace EventBooking.API.Models
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public Booking? Booking { get; set; }
    }
}