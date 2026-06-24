using System.ComponentModel.DataAnnotations;

namespace EventBooking.API.DTOs
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; } = 1;
    }
}