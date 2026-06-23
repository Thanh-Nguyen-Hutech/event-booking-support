namespace EventBooking.API.Models
{
    public class ServicePackage
    {
        public Guid PackageId { get; set; }
        public Guid VendorId { get; set; }
        public int CategoryId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public VendorProfile? VendorProfile { get; set; }
        public Category? Category { get; set; }
        public ICollection<AvailableSlot> AvailableSlots { get; set; } = new List<AvailableSlot>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}