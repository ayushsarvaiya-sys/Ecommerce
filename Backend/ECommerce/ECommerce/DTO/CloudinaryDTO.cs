namespace ECommerce.DTO
{
    public class PresignedUrlResponseDTO
    {
        public string? PresignedUrl { get; set; }
        public string? PublicId { get; set; }
        public long Timestamp { get; set; }
    }

    public class ImageUploadRequest
    {
        public string? ImageName { get; set; }
        public string? ResourceType { get; set; } = "image";
    }
}
