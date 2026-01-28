using ECommerce.Database;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.DTO;
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

            // Exclude deleted products for user endpoint
            query = query.Where(p => !p.IsDeleted);

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

        public async Task<(int totalCount, IEnumerable<ProductModel> products)> GetAllProductsPaginatedWithFilters(PaginationRequest request, bool isAdmin = false)
        {
            var query = _context.Products.AsQueryable();

            // Handle IsDeleted filtering
            // User endpoint: always exclude deleted
            // Admin endpoint: exclude deleted unless IncludeDeleted is explicitly true
            if (!isAdmin)
            {
                // User view: never show deleted - global query filter already handles this
                query = query.Where(p => !p.IsDeleted);
            }
            else
            {
                //query = query.IgnoreQueryFilters();

                // Admin view: check the IncludeDeleted flag
                if (request.IncludeDeleted)
                {
                    // Admin wants to see deleted: ignore the global query filter that excludes deleted products
                    //query = query.IgnoreQueryFilters();

                    query = query.IgnoreQueryFilters().Where(p => p.IsDeleted);
                }
                else
                {
                    // Admin doesn't want to see deleted: exclude them (respects global query filter)
                    //query = query.Where(p => !p.IsDeleted);

                    query = query.IgnoreQueryFilters();
                }
            }

            // Apply search filter (search by name or ID)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                // Try to parse as integer for ID search
                if (int.TryParse(request.SearchTerm, out int productId))
                {
                    // Search by both name and ID
                    query = query.Where(p => 
                        (p.Name != null && p.Name.Contains(request.SearchTerm)) || 
                        p.Id == productId
                    );
                }
                else
                {
                    // Search by name only
                    query = query.Where(p => p.Name != null && p.Name.Contains(request.SearchTerm));
                }
                query = query.IgnoreQueryFilters();
            }

            // Apply category filter
            if (request.CategoryId.HasValue && request.CategoryId > 0)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId);
            }

            // Apply price range filters
            if (request.MinPrice.HasValue && request.MinPrice >= 0)
            {
                query = query.Where(p => p.Price >= request.MinPrice);
            }

            if (request.MaxPrice.HasValue && request.MaxPrice >= 0)
            {
                query = query.Where(p => p.Price <= request.MaxPrice);
            }

            // Apply quantity filters (admin only)
            if (isAdmin)
            {
                if (request.MinQuantity.HasValue && request.MinQuantity >= 0)
                {
                    query = query.Where(p => p.Stock >= request.MinQuantity);
                }

                if (request.MaxQuantity.HasValue && request.MaxQuantity >= 0)
                {
                    query = query.Where(p => p.Stock <= request.MaxQuantity);
                }
            }

            var totalCount = await query.CountAsync();

            // Apply sorting
            if (isAdmin && !string.IsNullOrWhiteSpace(request.SortByPrice))
            {
                if (request.SortByPrice.ToLower() == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else if (request.SortByPrice.ToLower() == "desc")
                {
                    query = query.OrderByDescending(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }
            else if (isAdmin && !string.IsNullOrWhiteSpace(request.SortByQuantity))
            {
                if (request.SortByQuantity.ToLower() == "asc")
                {
                    query = query.OrderBy(p => p.Stock);
                }
                else if (request.SortByQuantity.ToLower() == "desc")
                {
                    query = query.OrderByDescending(p => p.Stock);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }

            Console.WriteLine("Query : ", query);
            Console.WriteLine("End");

            var products = await query
                .Include(p => p.Category)
                .Skip(request.Offset)
                .Take(request.Limit)
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

        public async Task<bool> RestoreProduct(int id)
        {
            //var product = await _context.Products.FindAsync(id);

            var product = await _context.Products
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return false;

            product.IsDeleted = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategory(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
        }
    }
}
