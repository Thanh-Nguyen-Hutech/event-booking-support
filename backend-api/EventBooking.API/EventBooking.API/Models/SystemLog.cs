namespace EventBooking.API.Models
{
    public class SystemLog
    {
        public Guid LogId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
