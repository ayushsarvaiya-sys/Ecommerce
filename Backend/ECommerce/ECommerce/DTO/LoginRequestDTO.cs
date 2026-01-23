using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
