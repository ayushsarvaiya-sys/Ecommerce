using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class ChangeCategoryNameRequest
    {
        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "New category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string? NewName { get; set; }
    }
}
