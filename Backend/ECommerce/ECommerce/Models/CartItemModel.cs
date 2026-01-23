using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class CartItemModel
    {
        [Key]
        [Column("CartItemId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cart is required")]
        public int CartId { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal PriceAtAddTime { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Foreign Key Navigation
        [ForeignKey("CartId")]
        public CartModel? Cart { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
    }
}
