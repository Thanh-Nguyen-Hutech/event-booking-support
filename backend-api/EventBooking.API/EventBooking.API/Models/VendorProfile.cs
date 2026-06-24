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
        public bool IsTemporarilyClosed { get; set; } = false;
        public ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public ICollection<RevenueReport> RevenueReports { get; set; } = new List<RevenueReport>();
    }

    public class VendorSchedule
    {
        public Guid ScheduleId { get; set; }
        public Guid VendorId { get; set; }
        public int DayOfWeek { get; set; } 
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }  
    }

    public class BlockedDate
    {
        public Guid BlockedId { get; set; }
        public Guid VendorId { get; set; }
        public DateTime Date { get; set; } 
        public string? Reason { get; set; } 
    }
}