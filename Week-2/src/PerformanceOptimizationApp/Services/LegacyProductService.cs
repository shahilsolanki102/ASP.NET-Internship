using PerformanceOptimizationApp.Data;
using PerformanceOptimizationApp.ViewModels;

namespace PerformanceOptimizationApp.Services
{
    /// <summary>
    /// LEGACY IMPLEMENTATION (Simulates typical unoptimized legacy ASP.NET code):
    /// Inefficiencies:
    /// 1. N+1 Queries: Performs a separate SQL query for each Category and Supplier inside a loop.
    /// 2. Synchronous Blocking: Uses synchronous methods causing thread starvation under load.
    /// 3. Memory Bloat: No AsNoTracking() - EF Core change tracker tracks every entity in RAM.
    /// 4. Client-side Filtering: Pulls entire datasets into application memory.
    /// 5. Missing Caching: Repeatedly queries the database for identical data.
    /// </summary>
    public class LegacyProductService : ILegacyProductService
    {
        private readonly PerformanceDbContext _context;

        public LegacyProductService(PerformanceDbContext context)
        {
            _context = context;
        }

        public List<ProductDto> GetProductsLegacy(int topCount = 100)
        {
            // Inefficiency 1: Fetching without projection or eager loading
            var products = _context.Products.Take(topCount).ToList();

            var result = new List<ProductDto>();

            // Inefficiency 2: N+1 Queries Problem! (Generates 200+ extra database round-trips)
            foreach (var prod in products)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == prod.CategoryId);
                var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == prod.SupplierId);

                result.Add(new ProductDto
                {
                    Id = prod.Id,
                    SKU = prod.SKU,
                    Name = prod.Name,
                    CategoryName = category?.Name ?? "N/A",
                    SupplierName = supplier?.CompanyName ?? "N/A",
                    UnitPrice = prod.UnitPrice,
                    UnitsInStock = prod.UnitsInStock,
                    IsDiscontinued = prod.IsDiscontinued
                });
            }

            return result;
        }

        public List<TopSellingReportDto> GetTopSellingReportLegacy()
        {
            // Inefficiency 3: Pulling thousands of OrderDetails into memory to aggregate in C# LINQ
            var allDetails = _context.OrderDetails.ToList();
            var allProducts = _context.Products.ToList();
            var allCategories = _context.Categories.ToList();
            var allSuppliers = _context.Suppliers.ToList();

            var report = allDetails
                .GroupBy(od => od.ProductId)
                .Select(g =>
                {
                    var prod = allProducts.FirstOrDefault(p => p.Id == g.Key);
                    var cat = allCategories.FirstOrDefault(c => c.Id == prod?.CategoryId);
                    var sup = allSuppliers.FirstOrDefault(s => s.Id == prod?.SupplierId);

                    return new TopSellingReportDto
                    {
                        ProductId = g.Key,
                        SKU = prod?.SKU ?? "N/A",
                        ProductName = prod?.Name ?? "N/A",
                        CategoryName = cat?.Name ?? "N/A",
                        SupplierName = sup?.CompanyName ?? "N/A",
                        TotalUnitsSold = g.Sum(x => x.Quantity),
                        TotalRevenueGenerated = g.Sum(x => x.LineTotal),
                        OrderCount = g.Select(x => x.OrderId).Distinct().Count()
                    };
                })
                .OrderByDescending(r => r.TotalRevenueGenerated)
                .Take(10)
                .ToList();

            return report;
        }
    }
}
