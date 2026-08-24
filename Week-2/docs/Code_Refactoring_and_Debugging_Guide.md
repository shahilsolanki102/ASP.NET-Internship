# Code Refactoring & Debugging Guide &bull; Week 2

This guide outlines the technical analysis, root-cause diagnostics, and refactoring patterns applied during the Week 2 performance tuning milestone.

---

## 1. The N+1 Query Problem & Resolution

### Legacy Bottleneck
```csharp
// Unoptimized Legacy: Loop generates 201 individual database round-trips
var products = _context.Products.Take(100).ToList();
foreach (var p in products)
{
    var category = _context.Categories.FirstOrDefault(c => c.Id == p.CategoryId);
    var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == p.SupplierId);
    // ...
}
```

### Refactored Pattern
```csharp
// Optimized: 1 single SQL query via LINQ DTO projection and AsNoTracking
return await _context.Products
    .AsNoTracking()
    .Take(100)
    .Select(p => new ProductDto
    {
        Id = p.Id,
        SKU = p.SKU,
        Name = p.Name,
        CategoryName = p.Category != null ? p.Category.Name : "N/A",
        SupplierName = p.Supplier != null ? p.Supplier.CompanyName : "N/A",
        UnitPrice = p.UnitPrice,
        UnitsInStock = p.UnitsInStock,
        IsDiscontinued = p.IsDiscontinued
    })
    .ToListAsync(cancellationToken);
```

---

## 2. In-Memory Aggregations vs. Server-Side SQL Aggregations

### Legacy Bottleneck
Loading 12,000+ transactional `OrderDetails` rows into RAM and grouping them in C# LINQ causes severe memory bloat and latency spikes (1,250ms).

### Refactored Pattern
```csharp
return await _context.OrderDetails
    .AsNoTracking()
    .Where(od => od.Order != null && od.Order.Status == "Completed")
    .GroupBy(od => new { od.ProductId, od.Product.SKU, od.Product.Name, CategoryName = od.Product.Category.Name, SupplierName = od.Product.Supplier.CompanyName })
    .Select(g => new TopSellingReportDto
    {
        ProductId = g.Key.ProductId,
        SKU = g.Key.SKU,
        ProductName = g.Key.Name,
        TotalUnitsSold = g.Sum(x => x.Quantity),
        TotalRevenueGenerated = g.Sum(x => x.LineTotal),
        OrderCount = g.Select(x => x.OrderId).Distinct().Count()
    })
    .OrderByDescending(r => r.TotalRevenueGenerated)
    .Take(10)
    .ToListAsync(cancellationToken);
```

---

## 3. SQL Server Index Tuning
```sql
-- Covering Index for Products
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId_SupplierId] 
ON [dbo].[Products] ([CategoryId] ASC, [SupplierId] ASC)
INCLUDE ([Name], [UnitPrice], [UnitsInStock], [IsDiscontinued]);

-- Covering Index for Date & Status Range Queries
CREATE NONCLUSTERED INDEX [IX_Orders_OrderDate_Status] 
ON [dbo].[Orders] ([OrderDate] DESC, [Status] ASC)
INCLUDE ([TotalAmount], [CustomerName]);
```
