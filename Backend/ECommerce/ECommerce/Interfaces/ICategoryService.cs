using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDTO> AddCategoryService(CreateCategoryDTO category);
        Task<CategoryResponseDTO?> GetCategoryByIdService(int id);
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesService();
        Task<CategoryResponseDTO> ChangeCategoryNameService(ChangeCategoryNameRequest request);
        Task<bool> DeleteCategoryService(int id);
    }
}
