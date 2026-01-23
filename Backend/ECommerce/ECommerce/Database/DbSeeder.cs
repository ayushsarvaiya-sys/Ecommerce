using ECommerce.Models;

namespace ECommerce.Database
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(EcommerceDbContext context)
        {
            if (!context.Users.Any())
            {
                var admin = new UserModel
                {
                    FullName = "Admin",
                    Email = "admin@ecommerce.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Admin@123", 13),
                    Role = "Admin",
                    IsDeleted = false
                };

                var user = new UserModel
                {
                    FullName = "User",
                    Email = "user@ecommerce.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("User@123", 13),
                    Role = "User",
                    IsDeleted = false
                };

                context.Users.Add(admin);
                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }
    }
}