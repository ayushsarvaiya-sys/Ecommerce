using ECommerce.Database;
using ECommerce.Interfaces;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDbContext _context;

        public CartRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<CartModel?> GetCartByUserId(int userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        public async Task<CartModel> CreateCart(int userId)
        {
            var cart = new CartModel
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartModel> AddToCart(int userId, int productId, int quantity)
        {
            // Get or create cart
            var cart = await GetCartByUserId(userId);
            if (cart == null)
            {
                cart = await CreateCart(userId);
            }

            // Get product to validate and get price
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (product.Stock < quantity)
                throw new InvalidOperationException($"Not enough stock available. Available: {product.Stock}");

            // Check if product already in cart
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId && !ci.IsDeleted);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.IsDeleted = false;
            }
            else
            {
                var newCartItem = new CartItemModel
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    PriceAtAddTime = product.Price ?? 0,
                    IsDeleted = false
                };
                _context.CartItems.Add(newCartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return cart with items
            return await GetCartWithItems(userId) ?? cart;
        }

        public async Task<CartModel?> UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id && !ci.IsDeleted);

            if (cartItem == null)
                throw new ArgumentException("Cart item not found");

            // Validate stock
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
            if (product != null && product.Stock < quantity)
                throw new InvalidOperationException($"Not enough stock available. Available: {product.Stock}");

            cartItem.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCartWithItems(userId);
        }

        public async Task<bool> RemoveCartItem(int userId, int cartItemId)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null)
                return false;

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id && !ci.IsDeleted);

            if (cartItem == null)
                return false;

            cartItem.IsDeleted = true;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(int userId)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null)
                return false;

            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id && !ci.IsDeleted)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.IsDeleted = true;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartModel?> GetCartWithItems(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }
    }
}
