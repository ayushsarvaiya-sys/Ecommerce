namespace ECommerce.Models
{
    public class ChatMessageModel
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UserProfileImage { get; set; }
    }
}
