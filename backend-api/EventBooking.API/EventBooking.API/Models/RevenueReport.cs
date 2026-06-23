namespace EventBooking.API.Models
{
    public class RevenueReport
    {
        public Guid ReportId { get; set; }
        public Guid VendorId { get; set; }
        public decimal TotalRevenue { get; set; }
        public int SuccessBookingCount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public VendorProfile? VendorProfile { get; set; }
    }
}
