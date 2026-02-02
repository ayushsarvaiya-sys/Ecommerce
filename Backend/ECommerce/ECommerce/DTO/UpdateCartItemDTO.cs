using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class UpdateCartItemDTO
    {
        [Required(ErrorMessage = "Cart Item ID is required")]
        public int CartItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
