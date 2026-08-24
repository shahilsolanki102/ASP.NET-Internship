-- ============================================================================
-- Script: 03_seed_benchmark_dataset.sql
-- Description: Generates realistic large dataset (Categories, Suppliers, Products,
--              Orders, OrderDetails) for benchmarking and load testing.
-- Author: Shahil (ASP.NET Internship - Week 2 Performance Tuning)
-- ============================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE [PerformanceTuningDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;

-- 1. Seed Categories (if empty)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories])
BEGIN
    INSERT INTO [dbo].[Categories] ([Name], [Description])
    VALUES 
        ('Electronics & Hardware', 'High-performance servers, processors, and computer hardware.'),
        ('Cloud Infrastructure', 'Networking, managed routers, and cloud appliances.'),
        ('Developer Peripherals', 'Keyboards, ergonomic mice, and multi-monitor setups.'),
        ('Storage & Backup', 'High-speed NVMe drives, SAN storage, and backup arrays.'),
        ('Software Licenses', 'Enterprise IDE licenses, OS distributions, and security tools.'),
        ('Audio & Video Equipment', 'Studio microphones, noise-canceling headsets, webcams.'),
        ('Office Automation', 'Smart office controllers, energy monitors, and power backup.'),
        ('Security & Biometrics', 'Smart card readers, biometric scanners, and access tokens.');
    PRINT 'Categories seeded.';
END
GO

-- 2. Seed Suppliers (if empty)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Suppliers])
BEGIN
    INSERT INTO [dbo].[Suppliers] ([CompanyName], [ContactName], [City], [Country], [Phone])
    VALUES 
        ('TechData Global Corp', 'Robert Henderson', 'San Jose', 'USA', '+1 408 555 0192'),
        ('Nordic Silicon AB', 'Astrid Lindgren', 'Stockholm', 'Sweden', '+46 8 555 0234'),
        ('Pacific Hardware Ltd', 'Kenji Sato', 'Tokyo', 'Japan', '+81 3 5555 0188'),
        ('Apex Systems India', 'Rajesh Sharma', 'Bangalore', 'India', '+91 80 5550 4912'),
        ('Helios Micro Devices', 'Elena Rossi', 'Milan', 'Italy', '+39 02 5550 8291'),
        ('Quantum Dynamics GmbH', 'Maximilian Weber', 'Munich', 'Germany', '+49 89 5550 1192');
    PRINT 'Suppliers seeded.';
END
GO

-- 3. Seed 1,500 Products using Set-Based Loop
IF (SELECT COUNT(*) FROM [dbo].[Products]) < 1000
BEGIN
    PRINT 'Seeding 1,500 Products for benchmarking...';
    
    DECLARE @catCount INT = (SELECT COUNT(*) FROM Categories);
    DECLARE @supCount INT = (SELECT COUNT(*) FROM Suppliers);

    ;WITH Numbers AS (
        SELECT TOP (1500) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
        FROM sys.all_objects a CROSS JOIN sys.all_objects b
    )
    INSERT INTO [dbo].[Products] (
        [CategoryId], [SupplierId], [SKU], [Name], [Description],
        [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [IsDiscontinued],
        [CreatedAt], [UpdatedAt]
    )
    SELECT 
        ((N % @catCount) + 1) AS CategoryId,
        ((N % @supCount) + 1) AS SupplierId,
        CONCAT('SKU-PRD-', RIGHT(CONCAT('00000', N), 5)) AS SKU,
        CONCAT('Enterprise Hardware Component Gen-', N, ' Model #', (N * 17) % 999) AS Name,
        CONCAT('High reliability grade component engineered for 24/7 mission critical operations. Generation batch #', N) AS Description,
        CAST((25.50 + ((N * 37) % 4500)) AS DECIMAL(18,2)) AS UnitPrice,
        ((N * 13) % 400) AS UnitsInStock,
        ((N * 7) % 50) AS UnitsOnOrder,
        20 AS ReorderLevel,
        CASE WHEN N % 25 = 0 THEN 1 ELSE 0 END AS IsDiscontinued,
        DATEADD(DAY, -(N % 365), SYSUTCDATETIME()) AS CreatedAt,
        SYSUTCDATETIME() AS UpdatedAt
    FROM Numbers;

    PRINT '1,500 Products seeded successfully.';
END
GO

-- 4. Seed 3,000 Orders and 12,000 OrderDetails
IF (SELECT COUNT(*) FROM [dbo].[Orders]) < 2000
BEGIN
    PRINT 'Seeding 3,000 Orders and OrderDetails for benchmarking...';

    ;WITH OrderNumbers AS (
        SELECT TOP (3000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
        FROM sys.all_objects a CROSS JOIN sys.all_objects b
    )
    INSERT INTO [dbo].[Orders] (
        [OrderNumber], [OrderDate], [CustomerName], [CustomerEmail],
        [ShippingCity], [ShippingCountry], [TotalAmount], [Status]
    )
    SELECT 
        CONCAT('ORD-2026-', RIGHT(CONCAT('00000', N), 6)) AS OrderNumber,
        DATEADD(MINUTE, -(N * 120), SYSUTCDATETIME()) AS OrderDate,
        CONCAT('Enterprise Client #', N) AS CustomerName,
        CONCAT('client', N, '@corporate.com') AS CustomerEmail,
        CASE N % 5 
            WHEN 0 THEN 'New York' 
            WHEN 1 THEN 'London' 
            WHEN 2 THEN 'Frankfurt' 
            WHEN 3 THEN 'Tokyo' 
            ELSE 'Singapore' 
        END AS ShippingCity,
        CASE N % 5 
            WHEN 0 THEN 'USA' 
            WHEN 1 THEN 'UK' 
            WHEN 2 THEN 'Germany' 
            WHEN 3 THEN 'Japan' 
            ELSE 'Singapore' 
        END AS ShippingCountry,
        0 AS TotalAmount,
        CASE WHEN N % 30 = 0 THEN 'Cancelled' ELSE 'Completed' END AS Status
    FROM OrderNumbers;

    -- Seed OrderDetails (4 items per order = 12,000 rows)
    DECLARE @prodCount INT = (SELECT COUNT(*) FROM Products);

    INSERT INTO [dbo].[OrderDetails] ([OrderId], [ProductId], [UnitPrice], [Quantity], [Discount])
    SELECT 
        o.Id AS OrderId,
        ((o.Id * 7 + item.N) % @prodCount) + 1 AS ProductId,
        CAST((45.00 + ((o.Id * 11) % 500)) AS DECIMAL(18,2)) AS UnitPrice,
        ((o.Id + item.N) % 8) + 1 AS Quantity,
        CASE WHEN o.Id % 4 = 0 THEN 0.05 ELSE 0.00 END AS Discount
    FROM [dbo].[Orders] o
    CROSS JOIN (VALUES (1), (2), (3), (4)) AS item(N);

    -- Update Order Total Amounts
    UPDATE o
    SET o.TotalAmount = ISNULL(sub.Total, 0)
    FROM [dbo].[Orders] o
    INNER JOIN (
        SELECT OrderId, SUM(LineTotal) AS Total
        FROM [dbo].[OrderDetails]
        GROUP BY OrderId
    ) sub ON o.Id = sub.OrderId;

    PRINT 'Orders & OrderDetails seeded successfully.';
END
GO

PRINT '=== Benchmark Dataset Ready: ===';
SELECT 'Categories' AS [Table], COUNT(*) AS [Rows] FROM [dbo].[Categories]
UNION ALL
SELECT 'Suppliers', COUNT(*) FROM [dbo].[Suppliers]
UNION ALL
SELECT 'Products', COUNT(*) FROM [dbo].[Products]
UNION ALL
SELECT 'Orders', COUNT(*) FROM [dbo].[Orders]
UNION ALL
SELECT 'OrderDetails', COUNT(*) FROM [dbo].[OrderDetails];
GO
