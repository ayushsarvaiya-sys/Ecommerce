using AutoMapper;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDTO> AddCategoryService(CreateCategoryDTO category)
        {
            var categoryModel = _mapper.Map<CategoryModel>(category);
            var result = await _categoryRepository.AddCategory(categoryModel);
            return _mapper.Map<CategoryResponseDTO>(result);
        }

        public async Task<CategoryResponseDTO?> GetCategoryByIdService(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            return category == null ? null : _mapper.Map<CategoryResponseDTO>(category);
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesService()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
        }

        public async Task<CategoryResponseDTO> ChangeCategoryNameService(ChangeCategoryNameRequest request)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(request.CategoryId);
            if (existingCategory == null)
                throw new ArgumentException("Category not found");

            existingCategory.Name = request.NewName;
            var result = await _categoryRepository.UpdateCategory(existingCategory);
            return _mapper.Map<CategoryResponseDTO>(result);
        }

        public async Task<bool> DeleteCategoryService(int id)
        {
            return await _categoryRepository.DeleteCategory(id);
        }
    }
}
