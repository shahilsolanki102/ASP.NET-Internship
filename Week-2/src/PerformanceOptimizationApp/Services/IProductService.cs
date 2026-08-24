using PerformanceOptimizationApp.Models;
using PerformanceOptimizationApp.ViewModels;

namespace PerformanceOptimizationApp.Services
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetProductsOptimizedAsync(
            int pageIndex = 1,
            int pageSize = 20,
            string? search = null,
            int? categoryId = null,
            CancellationToken cancellationToken = default);

        Task<List<ProductDto>> GetProductsBenchmarkOptimizedAsync(int topCount = 100, CancellationToken cancellationToken = default);

        Task<List<TopSellingReportDto>> GetTopSellingReportOptimizedAsync(int topN = 10, CancellationToken cancellationToken = default);

        Task<List<Category>> GetCachedCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
