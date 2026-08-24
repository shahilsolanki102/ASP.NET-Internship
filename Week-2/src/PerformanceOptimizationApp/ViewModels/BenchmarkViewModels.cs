namespace PerformanceOptimizationApp.ViewModels
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsDiscontinued { get; set; }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public int PageSize { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    public class BenchmarkResultViewModel
    {
        public string TestName { get; set; } = string.Empty;
        public string ScenarioDescription { get; set; } = string.Empty;

        // Legacy / Unoptimized Metrics
        public double LegacyExecutionTimeMs { get; set; }
        public double LegacyMemoryAllocatedMb { get; set; }
        public int LegacyDatabaseQueryCount { get; set; }
        public string LegacyIssuesFound { get; set; } = string.Empty;

        // Optimized / Refactored Metrics
        public double OptimizedExecutionTimeMs { get; set; }
        public double OptimizedMemoryAllocatedMb { get; set; }
        public int OptimizedDatabaseQueryCount { get; set; }
        public string OptimizationTechniquesUsed { get; set; } = string.Empty;

        // Calculated Improvements
        public double PerformanceGainPercentage => LegacyExecutionTimeMs > 0 
            ? Math.Round(((LegacyExecutionTimeMs - OptimizedExecutionTimeMs) / LegacyExecutionTimeMs) * 100, 1) 
            : 0;

        public double MemorySavingsPercentage => LegacyMemoryAllocatedMb > 0 
            ? Math.Round(((LegacyMemoryAllocatedMb - OptimizedMemoryAllocatedMb) / LegacyMemoryAllocatedMb) * 100, 1) 
            : 0;

        public double QueryReductionPercentage => LegacyDatabaseQueryCount > 0
            ? Math.Round(((double)(LegacyDatabaseQueryCount - OptimizedDatabaseQueryCount) / LegacyDatabaseQueryCount) * 100, 1)
            : 0;
    }

    public class TopSellingReportDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
        public int OrderCount { get; set; }
    }
}
