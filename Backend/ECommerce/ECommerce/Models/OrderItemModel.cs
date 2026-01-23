using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class OrderItemModel
    {
        [Key]
        [Column("OrderItemId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Foreign Key Navigation
        [ForeignKey("OrderId")]
        public OrderModel? Order { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
    }
}
