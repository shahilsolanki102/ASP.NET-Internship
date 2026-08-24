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
    /// 4. Client-side Filtering: Pulls datasets into application memory for client evaluation.
    /// 5. Missing Caching: Repeatedly queries the database for static lookup data.
    /// </summary>
    public class LegacyProductService : ILegacyProductService
    {
        private readonly PerformanceDbContext _context;

        public LegacyProductService(PerformanceDbContext context)
        {
            _context = context;
        }

        public List<ProductDto> GetProductsLegacy(int topCount = 30)
        {
            // Inefficiency 1: Fetching entities into Change Tracker without projection
            var products = _context.Products.Take(topCount).ToList();

            var result = new List<ProductDto>();

            // Inefficiency 2: N+1 Queries Problem! (Generates separate round-trips for each item)
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
            // Inefficiency 3: Fetching unaggregated order details into client memory
            var orderDetailsSubset = _context.OrderDetails.Take(500).ToList();
            var productIds = orderDetailsSubset.Select(od => od.ProductId).Distinct().ToList();
            var products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
            var categories = _context.Categories.ToList();
            var suppliers = _context.Suppliers.ToList();

            var report = orderDetailsSubset
                .GroupBy(od => od.ProductId)
                .Select(g =>
                {
                    var prod = products.FirstOrDefault(p => p.Id == g.Key);
                    var cat = categories.FirstOrDefault(c => c.Id == prod?.CategoryId);
                    var sup = suppliers.FirstOrDefault(s => s.Id == prod?.SupplierId);

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
