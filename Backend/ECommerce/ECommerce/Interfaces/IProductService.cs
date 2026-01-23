using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDTO> AddProductService(CreateProductDTO product);
        Task<ProductResponseDTO?> GetProductByIdService(int id);
        Task<IEnumerable<ProductResponseDTO>> GetAllProductsService();
        Task<PaginatedResponse<ProductResponseDTO>> GetProductsPaginatedService(PaginationRequest request);
        Task<PaginatedResponse<AdminProductResponseDTO>> GetProductsAdminPaginatedService(PaginationRequest request);
        Task<ProductResponseDTO> UpdateProductService(ProductDTO product);
        Task<bool> DeleteProductService(int id);
        Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryService(int categoryId);
        Task<ProductResponseDTO> RestockProductService(RestockProductRequest request);
    }
}
