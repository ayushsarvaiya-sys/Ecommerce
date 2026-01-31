using CsvHelper;
using CsvHelper.Configuration;
using ECommerce.DTO;
using OfficeOpenXml;
using System.Globalization;

namespace ECommerce.Utils
{
    public class FileParsingHelper
    {
        // Static initializer to set EPPlus license for non-commercial use
        static FileParsingHelper()
        {
            try
            {
                // For EPPlus 8.4.1, set the license using SetNonCommercialPersonal
                // This must be called before any ExcelPackage instances are created
                ExcelPackage.License.SetNonCommercialPersonal("E-Commerce Development");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to set EPPlus license: {ex.Message}");
            }
        }

        public static async Task<List<BulkProductImportDTO>> ParseExcelFileAsync(IFormFile file)
        {
            var products = new List<BulkProductImportDTO>();

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                // EPPlus 8.4.1: Create ExcelPackage from stream
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        throw new ArgumentException("Excel file has no worksheets");

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount <= 1)
                        throw new ArgumentException("Excel file has no data");

                    // Parse from row 2 (row 1 is headers)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var product = new BulkProductImportDTO
                        {
                            Name = worksheet.Cells[row, 1].Value?.ToString(),
                            Description = worksheet.Cells[row, 2].Value?.ToString(),
                            ImageUrl = worksheet.Cells[row, 3].Value?.ToString(),
                            Price = ParseDecimal(worksheet.Cells[row, 4].Value),
                            Stock = ParseInt(worksheet.Cells[row, 5].Value),
                            CategoryName = worksheet.Cells[row, 6].Value?.ToString(),
                            IsAvailable = ParseBool(worksheet.Cells[row, 7].Value)
                        };

                        products.Add(product);
                    }
                }
            }

            return products;
        }

        public static Task<List<BulkProductImportDTO>> ParseCsvFileAsync(IFormFile file)
        {
            var products = new List<BulkProductImportDTO>();

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            using (var stream = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var product = new BulkProductImportDTO
                    {
                        Name = csv.GetField("Name"),
                        Description = csv.GetField("Description"),
                        ImageUrl = csv.GetField("ImageUrl"),
                        Price = ParseDecimal(csv.GetField("Price")),
                        Stock = ParseInt(csv.GetField("Stock")),
                        CategoryName = csv.GetField("CategoryName"),
                        IsAvailable = ParseBool(csv.GetField("IsAvailable"))
                    };

                    products.Add(product);
                }
            }

            return Task.FromResult(products);
        }

        public static async Task<List<BulkProductImportDTO>> ParseFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var fileName = file.FileName.ToLower();

            if (fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls"))
            {
                return await ParseExcelFileAsync(file);
            }
            else if (fileName.EndsWith(".csv"))
            {
                return await ParseCsvFileAsync(file);
            }
            else
            {
                throw new ArgumentException("Unsupported file format. Please use .xlsx, .xls, or .csv files");
            }
        }

        private static decimal? ParseDecimal(object? value)
        {
            if (value == null)
                return null;

            if (decimal.TryParse(value.ToString(), out var result))
                return result;

            return null;
        }

        private static int? ParseInt(object? value)
        {
            if (value == null)
                return null;

            if (int.TryParse(value.ToString(), out var result))
                return result;

            return null;
        }

        private static bool? ParseBool(object? value)
        {
            if (value == null)
                return null;

            var strValue = value.ToString()?.ToLower();
            if (strValue == "true" || strValue == "1" || strValue == "yes")
                return true;
            if (strValue == "false" || strValue == "0" || strValue == "no")
                return false;

            return null;
        }

        public static List<string> ValidateProducts(List<BulkProductImportDTO> products)
        {
            var errors = new List<string>();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                int rowNumber = i + 2; // Row number in file (1-indexed, starting from row 2)

                if (string.IsNullOrWhiteSpace(product.Name))
                    errors.Add($"Row {rowNumber}: Product name is required");

                if (string.IsNullOrWhiteSpace(product.Description))
                    errors.Add($"Row {rowNumber}: Description is required");

                if (string.IsNullOrWhiteSpace(product.ImageUrl))
                    errors.Add($"Row {rowNumber}: Image URL is required");
                else if (!IsValidUrl(product.ImageUrl))
                    errors.Add($"Row {rowNumber}: Invalid image URL format");

                if (product.Price == null || product.Price <= 0)
                    errors.Add($"Row {rowNumber}: Price is required and must be greater than 0");

                if (product.Stock == null || product.Stock < 0)
                    errors.Add($"Row {rowNumber}: Stock is required and cannot be negative");

                if (string.IsNullOrWhiteSpace(product.CategoryName))
                    errors.Add($"Row {rowNumber}: Category name is required");
            }

            return errors;
        }

        private static bool IsValidUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }
    }
}
