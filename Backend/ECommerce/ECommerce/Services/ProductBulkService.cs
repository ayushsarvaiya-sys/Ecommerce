using ECommerce.Database;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Utils;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

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

                // Fetch existing products by name for upsert logic
                var existingProductNames = _dbContext.Products
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .Select(p => p.Name)
                    .ToHashSet();

                // Separate products into insert and update lists
                var productsToInsert = new List<ProductModel>();
                var productsToUpdate = new List<ProductModel>();

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

                    // Check if product already exists by name
                    if (existingProductNames.Contains(product.Name!))
                    {
                        productsToUpdate.Add(productModel);
                    }
                    else
                    {
                        productsToInsert.Add(productModel);
                    }
                }

                // Bulk insert new products
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

                // Update existing products
                if (productsToUpdate.Count > 0)
                {
                    // Fetch the existing products from DB with their IDs
                    var existingProductsMap = _dbContext.Products
                        .AsNoTracking()
                        .Where(p => !p.IsDeleted && productsToUpdate.Select(pt => pt.Name).Contains(p.Name))
                        .ToDictionary(p => p.Name ?? "", p => p.Id);

                    // Set the IDs for update
                    foreach (var productToUpdate in productsToUpdate)
                    {
                        if (existingProductsMap.TryGetValue(productToUpdate.Name ?? "", out var existingId))
                        {
                            productToUpdate.Id = existingId;
                        }
                    }

                    var bulkConfig = new BulkConfig
                    {
                        BatchSize = 1000,
                        UseTempDB = false,
                        CalculateStats = true
                    };

                    await _dbContext.BulkUpdateAsync(productsToUpdate, bulkConfig);
                    response.TotalUpdated = productsToUpdate.Count;
                }

                response.Message = $"Successfully imported {response.TotalInserted} products, {response.TotalUpdated} updated. {response.TotalFailed} records failed due to validation errors.";

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
