namespace ECommerce.DTO
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int CurrentPageCount { get; set; }
        public bool HasMore { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
