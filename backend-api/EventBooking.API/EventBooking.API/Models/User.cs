namespace EventBooking.API.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Role? Role { get; set; }
        public VendorProfile? VendorProfile { get; set; } // Quan hệ 1-1
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
    }
}