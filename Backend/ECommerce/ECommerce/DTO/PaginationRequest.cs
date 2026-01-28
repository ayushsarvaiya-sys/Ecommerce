using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class PaginationRequest
    {
        [Range(0, long.MaxValue, ErrorMessage = "Offset must be greater than or equal to 0")]
        public int Offset { get; set; } = 0;

        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string? SearchTerm { get; set; }

        // Filter properties
        public int? CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
        public decimal? MaxPrice { get; set; }

        // Admin only filters
        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public string? SortByPrice { get; set; } // "asc" or "desc"

        public string? SortByQuantity { get; set; } // "asc" or "desc"

        public bool IncludeDeleted { get; set; } = false; // Admin only: to show deleted products
    }
}
