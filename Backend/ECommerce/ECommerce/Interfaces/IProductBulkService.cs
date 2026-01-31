using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface IProductBulkService
    {
        Task<BulkImportPreviewDTO> PreviewBulkImportAsync(IFormFile file);
        Task<BulkImportResponseDTO> BulkImportProductsAsync(IFormFile file);
    }
}
