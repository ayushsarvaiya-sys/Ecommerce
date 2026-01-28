using ECommerce.DTO;
using ECommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Get Cloudinary configuration for client-side unsigned upload
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetUploadConfig")]
        public IActionResult GetUploadConfig()
        {
            try
            {
                var config = new
                {
                    cloudName = _cloudinaryService.GetCloudName(),
                    uploadPreset = _cloudinaryService.GetUploadPreset()
                };

                return Ok(new ApiResponse<dynamic>(200, config, "Cloudinary config retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }
    }
}
