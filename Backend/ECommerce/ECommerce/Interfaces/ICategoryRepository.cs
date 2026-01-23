using ECommerce.Models;

namespace ECommerce.Interfaces
{
    public interface ICategoryRepository
    {
        Task<CategoryModel> AddCategory(CategoryModel category);
        Task<CategoryModel?> GetCategoryById(int id);
        Task<IEnumerable<CategoryModel>> GetAllCategories();
        Task<CategoryModel> UpdateCategory(CategoryModel category);
        Task<bool> DeleteCategory(int id);
    }
}
