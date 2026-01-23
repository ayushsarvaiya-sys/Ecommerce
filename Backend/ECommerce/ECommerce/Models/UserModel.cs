using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class UserModel
    {
        [Key]
        [Column("UserId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "FullName is Requird")]
        [StringLength(100)]
        public string? FullName { get; set; } = null;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; } = null;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$", ErrorMessage = "Password must contain at least one uppercase, one lowercase, one digit, and one special character.")]
        public string? Password { get; set; } = null;

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression(@"^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'")]
        public string? Role { get; set; } = null;

        // Soft Delete
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        // public ICollection<CartModel> Carts { get; set; } = new List<CartModel>();
        // public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}
