using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class RegistrationRequestDTO
    {
        [Required(ErrorMessage = "FullName is Requird")]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$", ErrorMessage = "Password must contain at least one uppercase, one lowercase, one digit, and one special character.")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression(@"^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'")]
        public string? Role { get; set; }
    }
}
