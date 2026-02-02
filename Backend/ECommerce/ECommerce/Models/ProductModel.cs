using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class ProductModel
    {
        [Key]
        [Column("ProductId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string? Name { get; set; } = null;

        [StringLength(500)]
        [Required(ErrorMessage = "Product description is required")]
        public string? Description { get; set; } = null;

        [Required(ErrorMessage = "Image URL is required")]  
        [Url(ErrorMessage = "Invalid image URL format")]
        public string? ImageUrl { get; set; } = null;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; } = null;

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; } = null;

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        public int? CategoryId { get; set; } 

        public bool IsDeleted { get; set; } = false;

        // Foreign Key Navigation
        [ForeignKey("CategoryId")]
        public CategoryModel? Category { get; set; }

        // Navigation Property
        public ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        // public ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }
}
