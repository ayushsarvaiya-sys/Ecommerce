namespace ECommerce.DTO
{
    public class BulkProductImportDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsAvailable { get; set; }
    }

    public class BulkImportPreviewDTO
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<BulkProductImportDTO> PreviewData { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    public class BulkImportResponseDTO
    {
        public int TotalInserted { get; set; }
        public int TotalUpdated { get; set; }
        public int TotalFailed { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public string Message { get; set; } = "";
    }

    public class BulkDeleteRequest
    {
        public List<int> ProductIds { get; set; } = new();
    }

    public class BulkDeleteResponseDTO
    {
        public int TotalDeleted { get; set; }
        public List<int> FailedIds { get; set; } = new();
        public string Message { get; set; } = "";
    }
}
