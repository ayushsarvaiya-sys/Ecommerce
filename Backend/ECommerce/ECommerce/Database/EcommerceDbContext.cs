using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Database
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {
        }   

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        // public DbSet<CartModel> Carts { get; set; }
        // public DbSet<CartItemModel> CartItems { get; set; }
        // public DbSet<OrderModel> Orders { get; set; }
        // public DbSet<OrderItemModel> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Global Query Filters for Soft Delete
            modelBuilder.Entity<UserModel>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<CategoryModel>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<ProductModel>().HasQueryFilter(m => !m.IsDeleted);
            // modelBuilder.Entity<CartModel>().HasQueryFilter(m => !m.IsDeleted);
            // modelBuilder.Entity<CartItemModel>().HasQueryFilter(m => !m.IsDeleted);
            // modelBuilder.Entity<OrderModel>().HasQueryFilter(m => !m.IsDeleted);
            // modelBuilder.Entity<OrderItemModel>().HasQueryFilter(m => !m.IsDeleted);

            // Relationships with Cascade Delete Behavior
            // User -> Cart (One-to-Many)
            // modelBuilder.Entity<CartModel>()
            //     .HasOne(c => c.User)
            //     .WithMany(u => u.Carts)
            //     .HasForeignKey(c => c.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // // User -> Order (One-to-Many)
            // modelBuilder.Entity<OrderModel>()
            //     .HasOne(o => o.User)
            //     .WithMany(u => u.Orders)
            //     .HasForeignKey(o => o.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Category -> Product (One-to-Many)
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // // Cart -> CartItem (One-to-Many)
            // modelBuilder.Entity<CartItemModel>()
            //     .HasOne(ci => ci.Cart)
            //     .WithMany(c => c.CartItems)
            //     .HasForeignKey(ci => ci.CartId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // // Product -> CartItem (One-to-Many)
            // modelBuilder.Entity<CartItemModel>()
            //     .HasOne(ci => ci.Product)
            //     .WithMany(p => p.CartItems)
            //     .HasForeignKey(ci => ci.ProductId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // // Order -> OrderItem (One-to-Many)
            // modelBuilder.Entity<OrderItemModel>()
            //     .HasOne(oi => oi.Order)
            //     .WithMany(o => o.OrderItems)
            //     .HasForeignKey(oi => oi.OrderId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // // Product -> OrderItem (One-to-Many)
            // modelBuilder.Entity<OrderItemModel>()
            //     .HasOne(oi => oi.Product)
            //     .WithMany(p => p.OrderItems)
            //     .HasForeignKey(oi => oi.ProductId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // Indexes for Users
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Indexes for Products
            modelBuilder.Entity<ProductModel>()
                .HasIndex(p => p.CategoryId);

            // // Indexes for Carts
            // modelBuilder.Entity<CartModel>()
            //     .HasIndex(c => c.UserId);

            // // Indexes for CartItems
            // modelBuilder.Entity<CartItemModel>()
            //     .HasIndex(ci => ci.CartId);
            // modelBuilder.Entity<CartItemModel>()
            //     .HasIndex(ci => ci.ProductId);

            // // Indexes for Orders
            // modelBuilder.Entity<OrderModel>()
            //     .HasIndex(o => o.UserId);
            // modelBuilder.Entity<OrderModel>()
            //     .HasIndex(o => o.Status);
            // modelBuilder.Entity<OrderModel>()
            //     .HasIndex(o => o.OrderDate);

            // // Indexes for OrderItems
            // modelBuilder.Entity<OrderItemModel>()
            //     .HasIndex(oi => oi.OrderId);
            // modelBuilder.Entity<OrderItemModel>()
            //     .HasIndex(oi => oi.ProductId);
        }
    }
}
