namespace EventBooking.API.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; }
        public Guid BookingId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Booking? Booking { get; set; }
    }
}