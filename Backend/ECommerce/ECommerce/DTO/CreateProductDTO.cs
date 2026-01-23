using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Product description is required")]
        [StringLength(500, ErrorMessage = "Product description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Product image URL is required")]
        [Url(ErrorMessage = "Invalid image URL format")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Precision(10, 2)]
        public decimal? Price { get; set; }


        [Required(ErrorMessage = "Product stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be valid")]
        public int? CategoryId { get; set; }
    }
}
