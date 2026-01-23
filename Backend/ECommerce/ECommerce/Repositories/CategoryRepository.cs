using ECommerce.Database;
using ECommerce.Interfaces;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EcommerceDbContext _context;

        public CategoryRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryModel> AddCategory(CategoryModel category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryModel?> GetCategoryById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<CategoryModel> UpdateCategory(CategoryModel category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return false;

            category.IsDeleted = true;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
