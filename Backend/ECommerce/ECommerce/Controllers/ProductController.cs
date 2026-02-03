using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductBulkService _productBulkService;

        public ProductController(IProductService productService, IProductBulkService productBulkService)
        {
            _productService = productService;
            _productBulkService = productBulkService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDTO product)
        {
            try
            {
                var result = await _productService.AddProductService(product);
                return Ok(new ApiResponse<ProductResponseDTO>(200, result, "Product added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var result = await _productService.GetProductByIdService(id);
                if (result == null)
                    return NotFound(new ApiError(404, "Product not found"));

                return Ok(new ApiResponse<ProductResponseDTO>(200, result, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var result = await _productService.GetAllProductsService();
                return Ok(new ApiResponse<IEnumerable<ProductResponseDTO>>(200, result, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetPaginated")]
        public async Task<IActionResult> GetProductsPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                // Validate page and pageSize
                if (page < 1)
                    page = 1;
                if (pageSize < 1)
                    pageSize = 10;
                if (pageSize > 100)
                    pageSize = 100;

                // Convert page and pageSize to offset
                int offset = (page - 1) * pageSize;
                var request = new PaginationRequest
                {
                    Offset = offset,
                    Limit = pageSize,
                    SearchTerm = search,
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    IncludeDeleted = false
                };
                var result = await _productService.GetProductsPaginatedService(request);

                return Ok(new ApiResponse<PaginatedResponse<ProductResponseDTO>>(200, result, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetPaginatedAdmin")]
        public async Task<IActionResult> GetProductsAdminPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? minQuantity = null,
            [FromQuery] int? maxQuantity = null,
            [FromQuery] string? sortByPrice = null,
            [FromQuery] string? sortByQuantity = null,
            [FromQuery] bool includeDeleted = false)
        {
            try
            {
                // Validate page and pageSize
                if (page < 1)
                    page = 1;
                if (pageSize < 1)
                    pageSize = 10;
                if (pageSize > 100)
                    pageSize = 100;

                // Convert page and pageSize to offset
                int offset = (page - 1) * pageSize;
                var request = new PaginationRequest
                {
                    Offset = offset,
                    Limit = pageSize,
                    SearchTerm = search,
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinQuantity = minQuantity,
                    MaxQuantity = maxQuantity,
                    SortByPrice = sortByPrice,
                    SortByQuantity = sortByQuantity,
                    IncludeDeleted = includeDeleted
                };
                var result = await _productService.GetProductsAdminPaginatedService(request);

                return Ok(new ApiResponse<PaginatedResponse<AdminProductResponseDTO>>(200, result, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO product)
        {
            try
            {
                var result = await _productService.UpdateProductService(product);
                return Ok(new ApiResponse<ProductResponseDTO>(200, result, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductService(id);
                if (!result)
                    return NotFound(new ApiError(404, "Product not found"));

                return Ok(new ApiResponse<bool>(200, result, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            try
            {
                var result = await _productService.RestoreProductService(id);
                if (!result)
                    return NotFound(new ApiError(404, "Product not found"));

                return Ok(new ApiResponse<bool>(200, result, "Product restored successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var result = await _productService.GetProductsByCategoryService(categoryId);
                return Ok(new ApiResponse<IEnumerable<ProductResponseDTO>>(200, result, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Restock")]
        public async Task<IActionResult> RestockProduct([FromBody] RestockProductRequest request)
        {
            try
            {
                var result = await _productService.RestockProductService(request);
                return Ok(new ApiResponse<ProductResponseDTO>(200, result, "Product restocked successfully"));
            }
            catch (ArgumentException ex)  // Business rule exceptions (Product not found, Invalid input)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
            catch (InvalidOperationException ex)  // Invalid operations
            {
                return StatusCode(422, new ApiError(422, ex.Message));
            }
            // Let other exceptions (NullRef, DB errors, etc.) bubble to GEH → 500
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("BulkImport/Preview")]
        public async Task<IActionResult> PreviewBulkImport(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new ApiError(400, "No file provided"));

                var result = await _productBulkService.PreviewBulkImportAsync(file);
                return Ok(new ApiResponse<BulkImportPreviewDTO>(200, result, "Preview generated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("BulkImport/Upload")]
        public async Task<IActionResult> BulkImportProducts(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new ApiError(400, "No file provided"));

                var result = await _productBulkService.BulkImportProductsAsync(file);
                return Ok(new ApiResponse<BulkImportResponseDTO>(200, result, result.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("BulkDelete")]
        public async Task<IActionResult> BulkDeleteProducts([FromBody] BulkDeleteRequest request)
        {
            try
            {
                if (request == null || request.ProductIds == null || request.ProductIds.Count == 0)
                    return BadRequest(new ApiError(400, "No product IDs provided"));

                var result = await _productService.BulkDeleteProductsAsync(request.ProductIds);
                return Ok(new ApiResponse<BulkDeleteResponseDTO>(200, result, result.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }
    }
}