using ECommerce.Interfaces;

namespace ECommerce.Services
{
    public class CompressionTestDataService : ICompressionTestDataService
    {
        private readonly List<dynamic> _largeData1MB;
        private readonly List<dynamic> _largeData5MB;
        private readonly List<dynamic> _largeData10MB;

        public CompressionTestDataService()
        {
            _largeData1MB = GenerateData1MB();
            _largeData5MB = GenerateData5MB();
            _largeData10MB = GenerateData10MB();
        }

        public List<dynamic> GetLargeData1MB() => _largeData1MB;
        public List<dynamic> GetLargeData5MB() => _largeData5MB;
        public List<dynamic> GetLargeData10MB() => _largeData10MB;

        private List<dynamic> GenerateData1MB()
        {
            var items = new List<dynamic>();
            
            // Generate ~1 MB of data
            for (int i = 0; i < 5000; i++)
            {
                items.Add(new
                {
                    Id = i,
                    ProductId = i % 100 + 1,
                    ProductName = $"Product {i % 100 + 1}",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " + 
                                  "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    Price = (i % 100 + 1) * 10.99,
                    Category = new { Id = i % 5 + 1, Name = $"Category {i % 5 + 1}" },
                    Stock = i % 50 + 1,
                    Rating = (i % 5) + 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    Tags = new[] { "tag1", "tag2", "tag3", "tag4", "tag5" },
                    Specifications = new
                    {
                        Color = "Black",
                        Size = "Large",
                        Material = "Premium Cotton",
                        Weight = "500g",
                        Dimensions = "10x20x30cm"
                    }
                });
            }

            return items;
        }

        private List<dynamic> GenerateData5MB()
        {
            var items = new List<dynamic>();

            // Generate ~5 MB of data
            for (int i = 0; i < 25000; i++)
            {
                items.Add(new
                {
                    Id = i,
                    ProductId = i % 100 + 1,
                    ProductName = $"Product {i % 100 + 1}",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                                  "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
                                  "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                    Price = (i % 100 + 1) * 10.99,
                    Category = new { Id = i % 5 + 1, Name = $"Category {i % 5 + 1}" },
                    Stock = i % 50 + 1,
                    Rating = (i % 5) + 1,
                    Discount = i % 10,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-(i / 2)),
                    Tags = new[] { "tag1", "tag2", "tag3", "tag4", "tag5", "tag6", "tag7" },
                    Specifications = new
                    {
                        Color = "Black",
                        Size = "Large",
                        Material = "Premium Cotton",
                        Weight = "500g",
                        Dimensions = "10x20x30cm",
                        Warranty = "2 years",
                        ShippingWeight = "600g",
                        ManufacturerCode = "MFG-" + i
                    },
                    Reviews = new[]
                    {
                        new { Rating = 5, Comment = "Excellent product!" },
                        new { Rating = 4, Comment = "Very good" },
                        new { Rating = 5, Comment = "Highly recommended" }
                    }
                });
            }

            return items;
        }

        private List<dynamic> GenerateData10MB()
        {
            var items = new List<dynamic>();

            // Generate ~10 MB of data
            for (int i = 0; i < 50000; i++)
            {
                items.Add(new
                {
                    Id = i,
                    ProductId = i % 100 + 1,
                    ProductName = $"Product {i % 100 + 1}",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                                  "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
                                  "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
                                  "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    Price = (i % 100 + 1) * 10.99,
                    Category = new { Id = i % 5 + 1, Name = $"Category {i % 5 + 1}" },
                    Stock = i % 50 + 1,
                    Rating = (i % 5) + 1,
                    Discount = i % 10,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-(i / 2)),
                    Tags = new[] { "tag1", "tag2", "tag3", "tag4", "tag5", "tag6", "tag7", "tag8", "tag9" },
                    Specifications = new
                    {
                        Color = "Black",
                        Size = "Large",
                        Material = "Premium Cotton",
                        Weight = "500g",
                        Dimensions = "10x20x30cm",
                        Warranty = "2 years",
                        ShippingWeight = "600g",
                        ManufacturerCode = "MFG-" + i,
                        BarCode = "BAR-" + (i * 1000),
                        SKU = "SKU-" + (i * 500)
                    },
                    Reviews = new[]
                    {
                        new { Rating = 5, Comment = "Excellent product, highly satisfied with purchase!" },
                        new { Rating = 4, Comment = "Very good quality and fast delivery" },
                        new { Rating = 5, Comment = "Highly recommended, worth every penny" }
                    },
                    RelatedProducts = new[] { i - 1, i - 2, i + 1, i + 2 }
                });
            }

            return items;
        }
    }
}
