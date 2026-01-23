using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class CategoryModel
    {
        [Key]
        [Column("CategoryId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50)]
        public string? Name { get; set; } = null;

        [StringLength(200)]
        public string? Description { get; set; } = null;

        public bool IsDeleted { get; set; } = false;

        // Navigation Property
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
