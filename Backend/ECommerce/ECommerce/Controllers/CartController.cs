using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetCart(userId);
                return Ok(new ApiResponse<CartResponseDTO>(200, cart, "Cart retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError(400, "Invalid request data"));

                var userId = GetUserId();
                var cart = await _cartService.AddToCart(userId, request);
                return Ok(new ApiResponse<CartResponseDTO>(200, cart, "Product added to cart successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpPut("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError(400, "Invalid request data"));

                var userId = GetUserId();
                var cart = await _cartService.UpdateCartItem(userId, request);
                return Ok(new ApiResponse<CartResponseDTO>(200, cart, "Cart item updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpDelete("RemoveCartItem/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.RemoveCartItem(userId, cartItemId);
                return Ok(new ApiResponse<CartResponseDTO>(200, cart, "Cart item removed successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.ClearCart(userId);
                return Ok(new ApiResponse<CartResponseDTO>(200, cart, "Cart cleared successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpGet("CartItemCount")]
        public async Task<IActionResult> GetCartItemCount()
        {
            try
            {
                var userId = GetUserId();
                var count = await _cartService.GetCartItemCount(userId);
                return Ok(new ApiResponse<int>(200, count, "Cart item count retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }
    }
}
