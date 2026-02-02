namespace ECommerce.DTO
{
    public class CartItemResponseDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtAddTime { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal TotalPrice => PriceAtAddTime * Quantity;
    }
}
