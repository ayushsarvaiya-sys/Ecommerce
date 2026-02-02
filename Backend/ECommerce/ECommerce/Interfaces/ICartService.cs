using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDTO> GetCart(int userId);
        Task<CartResponseDTO> AddToCart(int userId, AddToCartDTO request);
        Task<CartResponseDTO> UpdateCartItem(int userId, UpdateCartItemDTO request);
        Task<CartResponseDTO> RemoveCartItem(int userId, int cartItemId);
        Task<CartResponseDTO> ClearCart(int userId);
        Task<int> GetCartItemCount(int userId);
    }
}
