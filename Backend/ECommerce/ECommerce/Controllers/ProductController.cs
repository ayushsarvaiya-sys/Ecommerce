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
        public ProductController(IProductService productService)
        {
            _productService = productService;
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
        public async Task<IActionResult> GetProductsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
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
                    SearchTerm = search
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
        public async Task<IActionResult> GetProductsAdminPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
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
                    SearchTerm = search
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
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }
    }
}
