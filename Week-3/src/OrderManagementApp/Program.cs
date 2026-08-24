using Microsoft.EntityFrameworkCore;
using OrderManagementApp.Data;
using OrderManagementApp.Models;
using OrderManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context (Supports SQL Server with automatic In-Memory fallback for test isolation)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connectionString) && !builder.Environment.IsEnvironment("Testing"))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseInMemoryDatabase("OrderManagementTestDb");
    }
});

// Register Application Services
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add MVC with Razor Runtime Compilation
var mvcBuilder = builder.Services.AddControllersWithViews();
#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif

var app = builder.Build();

// Seed initial data if running on in-memory or empty database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    try
    {
        if (context.Database.IsInMemory() || !context.Customers.Any())
        {
            SeedInitialData(context);
        }
    }
    catch
    {
        // Fallback to in-memory seeding
        SeedInitialData(context);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Data Seeder Helper
static void SeedInitialData(OrderDbContext context)
{
    if (!context.Customers.Any())
    {
        context.Customers.AddRange(
            new Customer { Name = "Shahil Patel", Email = "shahil@intern.com", Tier = CustomerTier.VIP },
            new Customer { Name = "Enterprise Cloud Corp", Email = "billing@enterprisecloud.com", Tier = CustomerTier.Enterprise },
            new Customer { Name = "John Standard", Email = "john@example.com", Tier = CustomerTier.Standard }
        );
    }

    if (!context.Items.Any())
    {
        context.Items.AddRange(
            new Item { SKU = "DEV-MON-4K", Name = "Dell UltraSharp 27\" 4K Monitor", UnitPrice = 450.00m, StockQuantity = 25 },
            new Item { SKU = "DEV-KBD-MECH", Name = "Keychron Q1 Mechanical Keyboard", UnitPrice = 180.00m, StockQuantity = 40 },
            new Item { SKU = "DEV-MOUSE-ERG", Name = "Logitech MX Master 3S Wireless", UnitPrice = 99.00m, StockQuantity = 60 },
            new Item { SKU = "DEV-DOCK-TB4", Name = "Thunderbolt 4 Multi-Port Dock", UnitPrice = 220.00m, StockQuantity = 15 },
            new Item { SKU = "DEV-HEADSET-NC", Name = "Sony WH-1000XM5 Headset", UnitPrice = 340.00m, StockQuantity = 30 }
        );
    }

    if (!context.Coupons.Any())
    {
        context.Coupons.AddRange(
            new Coupon { Code = "WELCOME10", DiscountPercentage = 10, MinOrderAmount = 100, MaxDiscountAmount = 50, ExpiryDate = DateTime.UtcNow.AddMonths(6) },
            new Coupon { Code = "SAVE25", DiscountPercentage = 25, MinOrderAmount = 300, MaxDiscountAmount = 150, ExpiryDate = DateTime.UtcNow.AddMonths(3) },
            new Coupon { Code = "EXPIRED50", DiscountPercentage = 50, MinOrderAmount = 50, MaxDiscountAmount = 100, ExpiryDate = DateTime.UtcNow.AddDays(-10), IsActive = false }
        );
    }

    context.SaveChanges();
}

// Required for WebApplicationFactory in Integration Tests
public partial class Program { }
