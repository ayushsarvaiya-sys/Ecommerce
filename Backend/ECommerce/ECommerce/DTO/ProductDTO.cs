using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be valid")]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Product description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid image URL format")]
        public string? ImageUrl { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        public int? CategoryId { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
