using System.ComponentModel.DataAnnotations;

namespace EventBooking.API.DTOs
{
    public class CreateReviewDTO
    {
        public Guid BookingId { get; set; }

        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}