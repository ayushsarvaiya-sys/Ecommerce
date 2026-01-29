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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDTO category)
        {
            try
            {
                var result = await _categoryService.AddCategoryService(category);
                return Ok(new ApiResponse<CategoryResponseDTO>(200, result, "Category added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdService(id);
                if (result == null)
                    return NotFound(new ApiError(404, "Category not found"));

                return Ok(new ApiResponse<CategoryResponseDTO>(200, result, "Category retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesService();
                return Ok(new ApiResponse<IEnumerable<CategoryResponseDTO>>(200, result, "Categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllAdmin")]
        public async Task<IActionResult> GetAllCategoriesAdmin([FromQuery] bool includeDeleted = false)
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesAdminService(includeDeleted);
                return Ok(new ApiResponse<IEnumerable<CategoryResponseDTO>>(200, result, "Categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeName")]
        public async Task<IActionResult> ChangeCategoryName([FromBody] ChangeCategoryNameRequest request)
        {
            try
            {
                var result = await _categoryService.ChangeCategoryNameService(request);
                return Ok(new ApiResponse<CategoryResponseDTO>(200, result, "Category name changed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryService(id);
                if (!result)
                    return NotFound(new ApiError(404, "Category not found"));

                return Ok(new ApiResponse<bool>(200, result, "Category deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            try
            {
                var result = await _categoryService.RestoreCategoryService(id);
                if (!result)
                    return NotFound(new ApiError(404, "Category not found"));

                return Ok(new ApiResponse<bool>(200, result, "Category restored successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }
    }
}
