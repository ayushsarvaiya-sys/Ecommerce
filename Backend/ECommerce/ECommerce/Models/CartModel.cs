using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class CartModel
    {
        [Key]
        [Column("CartId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        // Foreign Key Navigation
        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        // Navigation Property
        public ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }
}
