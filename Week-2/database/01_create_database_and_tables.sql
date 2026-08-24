-- ============================================================================
-- Script: 01_create_database_and_tables.sql
-- Description: Creates PerformanceTuningDb database and relational tables.
-- Author: Shahil (ASP.NET Internship - Week 2 Performance Tuning)
-- ============================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PerformanceTuningDb')
BEGIN
    CREATE DATABASE [PerformanceTuningDb];
    PRINT 'Database PerformanceTuningDb created successfully.';
END
GO

USE [PerformanceTuningDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Categories Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Categories_IsActive] DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Categories_CreatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [dbo].[Categories] created.';
END
GO

-- 2. Suppliers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Suppliers] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CompanyName] NVARCHAR(150) NOT NULL,
        [ContactName] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(30) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Suppliers_CreatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [dbo].[Suppliers] created.';
END
GO

-- 3. Products Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CategoryId] INT NOT NULL,
        [SupplierId] INT NOT NULL,
        [SKU] NVARCHAR(50) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [UnitsInStock] INT NOT NULL CONSTRAINT [DF_Products_UnitsInStock] DEFAULT (0),
        [UnitsOnOrder] INT NOT NULL CONSTRAINT [DF_Products_UnitsOnOrder] DEFAULT (0),
        [ReorderLevel] INT NOT NULL CONSTRAINT [DF_Products_ReorderLevel] DEFAULT (10),
        [IsDiscontinued] BIT NOT NULL CONSTRAINT [DF_Products_IsDiscontinued] DEFAULT (0),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Products_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Products_UpdatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
        CONSTRAINT [FK_Products_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([Id])
    );
    PRINT 'Table [dbo].[Products] created.';
END
GO

-- 4. Orders Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderNumber] NVARCHAR(50) NOT NULL,
        [OrderDate] DATETIME2(7) NOT NULL,
        [CustomerName] NVARCHAR(150) NOT NULL,
        [CustomerEmail] NVARCHAR(150) NOT NULL,
        [ShippingCity] NVARCHAR(100) NULL,
        [ShippingCountry] NVARCHAR(100) NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Orders_TotalAmount] DEFAULT (0),
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Orders_Status] DEFAULT ('Completed'),

        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [dbo].[Orders] created.';
END
GO

-- 5. OrderDetails Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderDetails] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [Quantity] INT NOT NULL,
        [Discount] DECIMAL(5,2) NOT NULL CONSTRAINT [DF_OrderDetails_Discount] DEFAULT (0),
        [LineTotal] AS (CAST([UnitPrice] * [Quantity] * (1 - [Discount]) AS DECIMAL(18,2))) PERSISTED,

        CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
    );
    PRINT 'Table [dbo].[OrderDetails] created.';
END
GO
