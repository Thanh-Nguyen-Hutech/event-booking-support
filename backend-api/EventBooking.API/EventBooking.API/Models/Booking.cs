namespace EventBooking.API.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime EventDate { get; set; }
        public int Status { get; set; } = 0; // 0: Pending, 1: Deposited, 2: Completed, 3: Cancelled
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        public string? CustomerNote { get; set; } // Cho phép null

        // Navigation Properties
        public User? Customer { get; set; }
        public ServicePackage? ServicePackage { get; set; }
        public Payment? Payment { get; set; } // 1-1
        public Review? Review { get; set; }   // 1-1
    }
}