using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class OrderModel
    {
        [Key]
        [Column("OrderId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(@"^(Pending|Processing|Shipped|Delivered|Cancelled)$", 
            ErrorMessage = "Status must be: Pending, Processing, Shipped, Delivered, or Cancelled")]
        public string? Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? ShippingAddress { get; set; } = null;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? DeliveryDate { get; set; } = null;

        public bool IsDeleted { get; set; } = false;

        // Foreign Key Navigation
        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        // Navigation Property
        public ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }
}
