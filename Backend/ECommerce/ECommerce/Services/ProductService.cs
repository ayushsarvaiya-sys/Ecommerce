using AutoMapper;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Calculates display stock status for frontend
        /// If stock > 10: return "In Stock"
        /// If stock <= 10 and > 0: return actual stock number (e.g., "3")
        /// If stock = 0 or negative: return "Out of Stock"
        /// Frontend can display: "In Stock" | "Only {stockNumber} left!" | "Out of Stock"
        /// </summary>
        private string GetDisplayStock(int? realStock)
        {
            // Handle null, zero, or negative stock values
            if (realStock == null || realStock <= 0)
                return "Out of Stock";
            
            if (realStock > 10)
                return "In Stock";
            
            return realStock.Value.ToString(); // Return actual stock number for "Only X left!"
        }

        private ProductResponseDTO MapToResponseDTO(ProductModel product)
        {
            var response = _mapper.Map<ProductResponseDTO>(product);
            response.StockStatus = GetDisplayStock(product.Stock);
            response.CategoryName = product.Category?.Name;
            return response;
        }

        public async Task<ProductResponseDTO> AddProductService(CreateProductDTO product)
        {
            // validate category existence before adding product
            var categoryExists = await _categoryRepository.GetCategoryById(product.CategoryId!.Value);
            if (categoryExists == null)
                throw new ArgumentException("Category not found");
            
            var productModel = _mapper.Map<ProductModel>(product);
            var result = await _productRepository.AddProduct(productModel);
            
            // Reload to get category information
            var addedProduct = await _productRepository.GetProductById(result.Id);
            return MapToResponseDTO(addedProduct!);
        }

        public async Task<ProductResponseDTO?> GetProductByIdService(int id)
        {
            var product = await _productRepository.GetProductById(id);
            return product == null ? null : MapToResponseDTO(product);
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsService()
        {
            var products = await _productRepository.GetAllProducts();
            return products.Select(p => MapToResponseDTO(p)).ToList();
        }

        public async Task<PaginatedResponse<ProductResponseDTO>> GetProductsPaginatedService(PaginationRequest request)
        {
            if (request == null)
                throw new ArgumentException("Pagination request is required");

            // Validate parameters
            int offset = request.Offset;
            int limit = request.Limit;
            string? searchTerm = request.SearchTerm?.Trim();

            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative");

            if (limit < 1 || limit > 100)
                throw new ArgumentException("Limit must be between 1 and 100");

            // Get paginated products from repository with search
            var (totalCount, products) = await _productRepository.GetAllProductsPaginated(offset, limit, searchTerm);

            // Map to response DTOs
            var productDTOs = products.Select(p => MapToResponseDTO(p)).ToList();

            // Calculate pagination info
            int currentPageCount = productDTOs.Count;
            bool hasMore = (offset + limit) < totalCount;

            return new PaginatedResponse<ProductResponseDTO>
            {
                TotalCount = totalCount,
                Offset = offset,
                Limit = limit,
                CurrentPageCount = currentPageCount,
                HasMore = hasMore,
                Data = productDTOs
            };
        }

        private AdminProductResponseDTO MapToAdminResponseDTO(ProductModel product)
        {
            return _mapper.Map<AdminProductResponseDTO>(product);
        }

        public async Task<PaginatedResponse<AdminProductResponseDTO>> GetProductsAdminPaginatedService(PaginationRequest request)
        {
            if (request == null)
                throw new ArgumentException("Pagination request is required");

            // Validate parameters
            int offset = request.Offset;
            int limit = request.Limit;
            string? searchTerm = request.SearchTerm?.Trim();

            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative");

            if (limit < 1 || limit > 100)
                throw new ArgumentException("Limit must be between 1 and 100");

            // Get paginated products from repository with search
            var (totalCount, products) = await _productRepository.GetAllProductsPaginated(offset, limit, searchTerm);

            // Map to admin response DTOs
            var adminProductDTOs = products.Select(p => MapToAdminResponseDTO(p)).ToList();

            // Calculate pagination info
            int currentPageCount = adminProductDTOs.Count;
            bool hasMore = (offset + limit) < totalCount;

            return new PaginatedResponse<AdminProductResponseDTO>
            {
                TotalCount = totalCount,
                Offset = offset,
                Limit = limit,
                CurrentPageCount = currentPageCount,
                HasMore = hasMore,
                Data = adminProductDTOs
            };
        }

        public async Task<ProductResponseDTO> UpdateProductService(ProductDTO product)
        {
            var existingProduct = await _productRepository.GetProductById(product.Id);
            if (existingProduct == null)
                throw new ArgumentException("Product not found");

            var productModel = _mapper.Map<ProductModel>(product);
            var result = await _productRepository.UpdateProduct(productModel);
            
            // Reload to get category information
            var updatedProduct = await _productRepository.GetProductById(result.Id);
            return MapToResponseDTO(updatedProduct!);
        }

        public async Task<bool> DeleteProductService(int id)
        {
            return await _productRepository.DeleteProduct(id);
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryService(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategory(categoryId);
            return products.Select(p => MapToResponseDTO(p)).ToList();
        }

        public async Task<ProductResponseDTO> RestockProductService(RestockProductRequest request)
        {
            var existingProduct = await _productRepository.GetProductById(request.ProductId);
            if (existingProduct == null)
                throw new ArgumentException("Product not found");

            // Validate and sanitize stock before updating
            int currentStock = (existingProduct.Stock == null || existingProduct.Stock < 0) ? 0 : existingProduct.Stock.Value;
            existingProduct.Stock = currentStock + request.QuantityToAdd;
            
            var result = await _productRepository.UpdateProduct(existingProduct);
            
            // Reload to get category information
            var restockedProduct = await _productRepository.GetProductById(result.Id);
            return MapToResponseDTO(restockedProduct!);
        }
    }
}
