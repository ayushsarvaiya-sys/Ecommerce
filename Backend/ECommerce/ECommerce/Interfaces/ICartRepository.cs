using ECommerce.DTO;
using ECommerce.Models;

namespace ECommerce.Interfaces
{
    public interface ICartRepository
    {
        Task<CartModel?> GetCartByUserId(int userId);
        Task<CartModel> CreateCart(int userId);
        Task<CartModel> AddToCart(int userId, int productId, int quantity);
        Task<CartModel?> UpdateCartItem(int userId, int cartItemId, int quantity);
        Task<bool> RemoveCartItem(int userId, int cartItemId);
        Task<bool> ClearCart(int userId);
        Task<CartModel?> GetCartWithItems(int userId);
    }
}
