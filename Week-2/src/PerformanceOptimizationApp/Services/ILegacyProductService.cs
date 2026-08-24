using PerformanceOptimizationApp.ViewModels;

namespace PerformanceOptimizationApp.Services
{
    public interface ILegacyProductService
    {
        // Simulated Legacy: Synchronous, N+1 queries, No Pagination, No Cache
        List<ProductDto> GetProductsLegacy(int topCount = 100);
        List<TopSellingReportDto> GetTopSellingReportLegacy();
    }
}
