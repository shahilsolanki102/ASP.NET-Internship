using Microsoft.EntityFrameworkCore;
using OrderManagementApp.Models;

namespace OrderManagementApp.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
        public DbSet<CustomerOrderItem> CustomerOrderItems => Set<CustomerOrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Coupon>().ToTable("Coupons");
            modelBuilder.Entity<CustomerOrder>().ToTable("CustomerOrders");
            modelBuilder.Entity<CustomerOrderItem>().ToTable("CustomerOrderItems");

            modelBuilder.Entity<CustomerOrder>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}
