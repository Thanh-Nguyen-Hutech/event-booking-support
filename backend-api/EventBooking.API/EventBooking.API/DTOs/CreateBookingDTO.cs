namespace EventBooking.API.DTOs
{
    public class CreateBookingDTO
    {
        public Guid PackageId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime EventDate { get; set; }

        public string TimeSlot { get; set; } = string.Empty;

        public string EventLocation { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
    }
}