using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class RestockProductRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be valid")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Restock quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Restock quantity must be greater than 0")]
        public int QuantityToAdd { get; set; }
    }
}
