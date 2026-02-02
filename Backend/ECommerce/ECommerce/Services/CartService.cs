using AutoMapper;
using ECommerce.DTO;
using ECommerce.Interfaces;

namespace ECommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<CartResponseDTO> GetCart(int userId)
        {
            var cart = await _cartRepository.GetCartWithItems(userId);
            
            if (cart == null)
            {
                // Return empty cart
                return new CartResponseDTO
                {
                    UserId = userId,
                    CartItems = new List<CartItemResponseDTO>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            return MapToResponseDTO(cart);
        }

        public async Task<CartResponseDTO> AddToCart(int userId, AddToCartDTO request)
        {
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var cart = await _cartRepository.AddToCart(userId, request.ProductId, request.Quantity);
            return MapToResponseDTO(cart);
        }

        public async Task<CartResponseDTO> UpdateCartItem(int userId, UpdateCartItemDTO request)
        {
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var cart = await _cartRepository.UpdateCartItem(userId, request.CartItemId, request.Quantity);
            
            if (cart == null)
                throw new ArgumentException("Failed to update cart item");

            return MapToResponseDTO(cart);
        }

        public async Task<CartResponseDTO> RemoveCartItem(int userId, int cartItemId)
        {
            var removed = await _cartRepository.RemoveCartItem(userId, cartItemId);
            
            if (!removed)
                throw new ArgumentException("Failed to remove cart item");

            var cart = await _cartRepository.GetCartWithItems(userId);
            
            if (cart == null)
            {
                return new CartResponseDTO
                {
                    UserId = userId,
                    CartItems = new List<CartItemResponseDTO>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            return MapToResponseDTO(cart);
        }

        public async Task<CartResponseDTO> ClearCart(int userId)
        {
            var cleared = await _cartRepository.ClearCart(userId);
            
            if (!cleared)
                throw new ArgumentException("Failed to clear cart");

            return new CartResponseDTO
            {
                UserId = userId,
                CartItems = new List<CartItemResponseDTO>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<int> GetCartItemCount(int userId)
        {
            var cart = await _cartRepository.GetCartWithItems(userId);
            
            if (cart == null)
                return 0;

            return cart.CartItems.Sum(ci => ci.Quantity);
        }

        private CartResponseDTO MapToResponseDTO(Models.CartModel cart)
        {
            var response = new CartResponseDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                CartItems = new List<CartItemResponseDTO>()
            };

            if (cart.CartItems != null && cart.CartItems.Any())
            {
                response.CartItems = cart.CartItems
                    .Where(ci => !ci.IsDeleted)
                    .Select(ci => new CartItemResponseDTO
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        PriceAtAddTime = ci.PriceAtAddTime,
                        ProductName = ci.Product?.Name,
                        ProductImageUrl = ci.Product?.ImageUrl
                    })
                    .ToList();
            }

            return response;
        }
    }
}
