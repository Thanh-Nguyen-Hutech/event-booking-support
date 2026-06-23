namespace EventBooking.API.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<PermissionDetail> PermissionDetails { get; set; } = new List<PermissionDetail>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}