namespace ECommerce.DTO
{
    public class CartResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemResponseDTO> CartItems { get; set; } = new List<CartItemResponseDTO>();
        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);
        public int TotalItems => CartItems.Sum(item => item.Quantity);
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
