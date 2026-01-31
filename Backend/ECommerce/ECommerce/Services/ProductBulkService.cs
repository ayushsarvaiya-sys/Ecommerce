using ECommerce.Database;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Utils;
using EFCore.BulkExtensions;

namespace ECommerce.Services
{
    public class ProductBulkService : IProductBulkService
    {
        private readonly EcommerceDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;

        public ProductBulkService(EcommerceDbContext dbContext, ICategoryRepository categoryRepository)
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
        }

        public async Task<BulkImportPreviewDTO> PreviewBulkImportAsync(IFormFile file)
        {
            var preview = new BulkImportPreviewDTO();

            try
            {
                // Parse the file
                var parsedProducts = await FileParsingHelper.ParseFileAsync(file);
                preview.TotalRecords = parsedProducts.Count;

                // Validate products
                var validationErrors = FileParsingHelper.ValidateProducts(parsedProducts);
                preview.Errors = validationErrors;

                // Separate valid and invalid records
                var validProducts = new List<BulkProductImportDTO>();
                var invalidRowNumbers = new HashSet<int>();

                for (int i = 0; i < parsedProducts.Count; i++)
                {
                    var rowErrors = validationErrors.Where(e => e.StartsWith($"Row {i + 2}:")).ToList();
                    if (rowErrors.Count == 0)
                    {
                        validProducts.Add(parsedProducts[i]);
                    }
                    else
                    {
                        invalidRowNumbers.Add(i);
                    }
                }

                preview.ValidRecords = validProducts.Count;
                preview.InvalidRecords = preview.TotalRecords - preview.ValidRecords;

                // Show first 10 records for preview
                preview.PreviewData = validProducts.Take(10).ToList();

                return preview;
            }
            catch (Exception ex)
            {
                preview.Errors.Add($"Error processing file: {ex.Message}");
                return preview;
            }
        }

        public async Task<BulkImportResponseDTO> BulkImportProductsAsync(IFormFile file)
        {
            var response = new BulkImportResponseDTO();

            try
            {
                // Parse the file
                var parsedProducts = await FileParsingHelper.ParseFileAsync(file);

                // Validate products
                var validationErrors = FileParsingHelper.ValidateProducts(parsedProducts);

                // Separate valid and invalid records
                var validProducts = new List<BulkProductImportDTO>();
                var invalidRowNumbers = new HashSet<int>();

                for (int i = 0; i < parsedProducts.Count; i++)
                {
                    var rowErrors = validationErrors.Where(e => e.StartsWith($"Row {i + 2}:")).ToList();
                    if (rowErrors.Count == 0)
                    {
                        validProducts.Add(parsedProducts[i]);
                    }
                    else
                    {
                        invalidRowNumbers.Add(i);
                    }
                }

                if (validProducts.Count == 0)
                {
                    response.TotalFailed = parsedProducts.Count;
                    response.ErrorMessages = validationErrors;
                    response.Message = "No valid records found in the file";
                    return response;
                }

                // Fetch all categories to map category names to IDs
                var categories = await _categoryRepository.GetAllCategories();
                var categoryMap = categories.ToDictionary(c => c.Name!.ToLower(), c => c.Id);

                // Convert DTOs to ProductModel
                var productsToInsert = new List<ProductModel>();

                foreach (var product in validProducts)
                {
                    // Get category ID
                    var categoryKey = product.CategoryName?.ToLower();
                    if (string.IsNullOrWhiteSpace(categoryKey) || !categoryMap.TryGetValue(categoryKey, out var categoryId))
                    {
                        response.ErrorMessages.Add($"Category '{product.CategoryName}' not found");
                        response.TotalFailed++;
                        continue;
                    }

                    var productModel = new ProductModel
                    {
                        Name = product.Name,
                        Description = product.Description,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Stock = product.Stock,
                        CategoryId = categoryId,
                        IsAvailable = product.IsAvailable ?? true,
                        IsDeleted = false
                    };

                    productsToInsert.Add(productModel);
                }

                // Bulk insert using EFCore.BulkExtensions
                if (productsToInsert.Count > 0)
                {
                    var bulkConfig = new BulkConfig
                    {
                        BatchSize = 1000,
                        UseTempDB = false,
                        CalculateStats = true
                    };

                    await _dbContext.BulkInsertAsync(productsToInsert, bulkConfig);
                    response.TotalInserted = productsToInsert.Count;
                }

                response.Message = $"Successfully imported {response.TotalInserted} products. {response.TotalFailed} records failed due to validation errors.";

                return response;
            }
            catch (Exception ex)
            {
                response.TotalFailed = 1;
                response.ErrorMessages.Add($"Error during bulk import: {ex.Message}");
                response.Message = "Bulk import failed";
                return response;
            }
        }
    }
}
