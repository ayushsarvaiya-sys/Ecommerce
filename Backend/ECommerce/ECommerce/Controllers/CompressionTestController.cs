using ECommerce.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompressionTestController : ControllerBase
    {
        private readonly ICompressionTestDataService _dataService;

        public CompressionTestController(ICompressionTestDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Returns ~1 MB of pre-generated JSON data to test compression efficiency
        /// Data is generated once on application startup
        /// </summary>
        [HttpGet("large-response-1mb")]
        public IActionResult GetLargeResponse1MB()
        {
            var items = _dataService.GetLargeData1MB();

            return Ok(new
            {
                status = 200,
                message = "Large response for compression testing (~1 MB) - Pre-generated at startup",
                dataCount = items.Count,
                timestamp = DateTime.UtcNow,
                data = items
            });
        }

        /// <summary>
        /// Returns ~5 MB of pre-generated JSON data to test compression efficiency
        /// Data is generated once on application startup
        /// </summary>
        [HttpGet("large-response-5mb")]
        public IActionResult GetLargeResponse5MB()
        {
            var items = _dataService.GetLargeData5MB();

            return Ok(new
            {
                status = 200,
                message = "Large response for compression testing (~5 MB) - Pre-generated at startup",
                dataCount = items.Count,
                timestamp = DateTime.UtcNow,
                data = items
            });
        }

        /// <summary>
        /// Returns ~10 MB of pre-generated JSON data to test compression efficiency
        /// Data is generated once on application startup
        /// </summary>
        [HttpGet("large-response-10mb")]
        public IActionResult GetLargeResponse10MB()
        {
            var items = _dataService.GetLargeData10MB();

            return Ok(new
            {
                status = 200,
                message = "Large response for compression testing (~10 MB) - Pre-generated at startup",
                dataCount = items.Count,
                timestamp = DateTime.UtcNow,
                data = items
            });
        }
    }
}
