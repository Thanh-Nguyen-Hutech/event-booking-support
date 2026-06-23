namespace EventBooking.API.Models
{
    public class AvailableSlot
    {
        public Guid SlotId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime EventDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public ServicePackage? ServicePackage { get; set; }
    }
}