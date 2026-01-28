using ECommerce.Models;
using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductModel> AddProduct(ProductModel product);
        Task<ProductModel?> GetProductById(int id);
        Task<IEnumerable<ProductModel>> GetAllProducts();
        Task<(int totalCount, IEnumerable<ProductModel> products)> GetAllProductsPaginated(int offset, int limit, string? searchTerm = null);
        Task<(int totalCount, IEnumerable<ProductModel> products)> GetAllProductsPaginatedWithFilters(PaginationRequest request, bool isAdmin = false);
        Task<ProductModel> UpdateProduct(ProductModel product);
        Task<bool> DeleteProduct(int id);
        Task<bool> RestoreProduct(int id);
        Task<IEnumerable<ProductModel>> GetProductsByCategory(int categoryId);
    }
}
