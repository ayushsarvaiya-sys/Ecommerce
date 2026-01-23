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
    }
}
