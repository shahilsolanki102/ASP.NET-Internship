using Microsoft.EntityFrameworkCore;
using PerformanceOptimizationApp.Models;

namespace PerformanceOptimizationApp.Data
{
    public class PerformanceDbContext : DbContext
    {
        public PerformanceDbContext(DbContextOptions<PerformanceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mapping
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Supplier>().ToTable("Suppliers");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");

            // Indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.CategoryId, p.SupplierId })
                .HasDatabaseName("IX_Products_CategoryId_SupplierId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique()
                .HasDatabaseName("IX_Products_SKU");

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.OrderDate, o.Status })
                .HasDatabaseName("IX_Orders_OrderDate_Status");

            modelBuilder.Entity<OrderDetail>()
                .HasIndex(od => new { od.ProductId, od.OrderId })
                .HasDatabaseName("IX_OrderDetails_ProductId_OrderId");
        }
    }
}
