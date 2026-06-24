using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.API.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime EventDate { get; set; }

        // THÊM DÒNG NÀY VÀO ĐÂY:
        public string TimeSlot { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Status { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        public string? CustomerNote { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User? Customer { get; set; }

        [ForeignKey("PackageId")]
        public virtual ServicePackage? ServicePackage { get; set; }
        public Payment? Payment { get; set; }
        public Review? Review { get; set; }
    }
}