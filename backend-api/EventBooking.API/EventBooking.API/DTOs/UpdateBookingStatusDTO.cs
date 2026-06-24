namespace EventBooking.API.DTOs
{
    public class UpdateBookingStatusDTO
    {
        public int Status { get; set; }
        public string? Note { get; set; }
    }
}