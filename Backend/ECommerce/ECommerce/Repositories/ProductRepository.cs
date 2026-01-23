using ECommerce.Database;
using ECommerce.Interfaces;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDbContext _context;

        public ProductRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<ProductModel> AddProduct(ProductModel product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<ProductModel?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(int totalCount, IEnumerable<ProductModel> products)> GetAllProductsPaginated(int offset, int limit, string? searchTerm = null)
        {
            var query = _context.Products.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name != null && p.Name.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            
            var products = await query
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return (totalCount, products);
        }

        public async Task<IEnumerable<ProductModel>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<ProductModel> UpdateProduct(ProductModel product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return false;

            product.IsDeleted = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategory(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
