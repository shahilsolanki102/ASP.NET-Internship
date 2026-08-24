-- ============================================================================
-- Script: 02_performance_indexes_and_optimizations.sql
-- Description: Creates targeted B-Tree indexes and optimized stored procedures
--              to eliminate table scans and optimize multi-table joins.
-- Author: Shahil (ASP.NET Internship - Week 2 Performance Tuning)
-- ============================================================================

USE [PerformanceTuningDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Index on Products FKs and Search Columns (CategoryId, SupplierId, SKU, Price)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = N'IX_Products_CategoryId_SupplierId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryId_SupplierId] 
    ON [dbo].[Products] ([CategoryId] ASC, [SupplierId] ASC)
    INCLUDE ([Name], [UnitPrice], [UnitsInStock], [IsDiscontinued]);
    PRINT 'Index IX_Products_CategoryId_SupplierId created with covering columns.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = N'IX_Products_SKU')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_SKU] ON [dbo].[Products] ([SKU] ASC);
    PRINT 'Unique Index IX_Products_SKU created.';
END
GO

-- 2. Index on Orders Date and Status for Fast Analytical Range Queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = N'IX_Orders_OrderDate_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Orders_OrderDate_Status] 
    ON [dbo].[Orders] ([OrderDate] DESC, [Status] ASC)
    INCLUDE ([TotalAmount], [CustomerName]);
    PRINT 'Index IX_Orders_OrderDate_Status created with covering columns.';
END
GO

-- 3. Index on OrderDetails ProductId and OrderId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = N'IX_OrderDetails_ProductId_OrderId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductId_OrderId] 
    ON [dbo].[OrderDetails] ([ProductId] ASC, [OrderId] ASC)
    INCLUDE ([Quantity], [UnitPrice], [LineTotal]);
    PRINT 'Index IX_OrderDetails_ProductId_OrderId created.';
END
GO

-- 4. Optimized Stored Procedure: Top Selling Products with Aggregation Tuning
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTopSellingProductsReport]
    @TopN INT = 10,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @StartDate = COALESCE(@StartDate, DATEADD(YEAR, -2, SYSUTCDATETIME()));
    SET @EndDate = COALESCE(@EndDate, SYSUTCDATETIME());

    SELECT TOP (@TopN)
        p.[Id] AS ProductId,
        p.[SKU],
        p.[Name] AS ProductName,
        c.[Name] AS CategoryName,
        s.[CompanyName] AS SupplierName,
        SUM(od.[Quantity]) AS TotalUnitsSold,
        SUM(od.[LineTotal]) AS TotalRevenueGenerated,
        COUNT(DISTINCT od.[OrderId]) AS OrderCount
    FROM [dbo].[OrderDetails] od WITH (NOLOCK)
    INNER JOIN [dbo].[Orders] o WITH (NOLOCK) ON od.[OrderId] = o.[Id]
    INNER JOIN [dbo].[Products] p WITH (NOLOCK) ON od.[ProductId] = p.[Id]
    INNER JOIN [dbo].[Categories] c WITH (NOLOCK) ON p.[CategoryId] = c.[Id]
    INNER JOIN [dbo].[Suppliers] s WITH (NOLOCK) ON p.[SupplierId] = s.[Id]
    WHERE o.[OrderDate] BETWEEN @StartDate AND @EndDate
      AND o.[Status] = 'Completed'
    GROUP BY p.[Id], p.[SKU], p.[Name], c.[Name], s.[CompanyName]
    ORDER BY TotalRevenueGenerated DESC;
END
GO
