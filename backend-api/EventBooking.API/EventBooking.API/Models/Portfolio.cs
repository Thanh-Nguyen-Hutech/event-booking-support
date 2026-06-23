using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.API.Models
{
    public class Portfolio
    {
        public Guid PortfolioId { get; set; }
        public Guid VendorId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("VendorId")]
        public VendorProfile? VendorProfile { get; set; }
        public ICollection<PortfolioImage> Images { get; set; } = new List<PortfolioImage>();
    }
}