using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTO
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50)]
        public string? Name { get; set; } = null;

        [StringLength(200)]
        public string? Description { get; set; } = null;
    }
}
