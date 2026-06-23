namespace EventBooking.API.Models
{
    public class PermissionDetail
    {
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public string PermissionName { get; set; } = string.Empty;

        public Role? Role { get; set; }
    }
}