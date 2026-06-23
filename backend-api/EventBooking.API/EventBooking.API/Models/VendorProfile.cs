namespace EventBooking.API.Models
{
    public class VendorProfile
    {
        public Guid VendorId { get; set; }
        public Guid UserId { get; set; }
        public string StudioName { get; set; } = string.Empty;
        public double AverageRating { get; set; } = 0;

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public ICollection<RevenueReport> RevenueReports { get; set; } = new List<RevenueReport>();
    }
}