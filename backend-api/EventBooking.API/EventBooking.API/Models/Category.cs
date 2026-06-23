namespace EventBooking.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
    }
}