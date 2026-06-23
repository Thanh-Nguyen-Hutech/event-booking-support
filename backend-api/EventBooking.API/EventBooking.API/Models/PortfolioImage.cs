namespace EventBooking.API.Models
{
    public class PortfolioImage
    {
        public Guid ImageId { get; set; }
        public Guid PortfolioId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public Portfolio? Portfolio { get; set; }
    }
}
