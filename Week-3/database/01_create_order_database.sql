-- ============================================================================
-- Script: 01_create_order_database.sql
-- Description: Creates OrderManagementDb schema and seeds test data.
-- Author: Shahil (ASP.NET Internship - Week 3 Automated Testing)
-- ============================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'OrderManagementDb')
BEGIN
    CREATE DATABASE [OrderManagementDb];
    PRINT 'Database OrderManagementDb created successfully.';
END
GO

USE [OrderManagementDb];
GO

-- 1. Customers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customers] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [Tier] INT NOT NULL CONSTRAINT [DF_Customers_Tier] DEFAULT (0),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Customers_CreatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 2. Items Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Items] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SKU] NVARCHAR(50) NOT NULL,
        [Name] NVARCHAR(150) NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [StockQuantity] INT NOT NULL CONSTRAINT [DF_Items_StockQuantity] DEFAULT (0),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Items_IsActive] DEFAULT (1),

        CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 3. Coupons Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coupons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Coupons] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Code] NVARCHAR(30) NOT NULL,
        [DiscountPercentage] DECIMAL(18,2) NOT NULL,
        [MaxDiscountAmount] DECIMAL(18,2) NOT NULL,
        [MinOrderAmount] DECIMAL(18,2) NOT NULL,
        [ExpiryDate] DATETIME2(7) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Coupons_IsActive] DEFAULT (1),

        CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 4. CustomerOrders Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerOrders] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderNumber] NVARCHAR(50) NOT NULL,
        [CustomerId] INT NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL,
        [DiscountAmount] DECIMAL(18,2) NOT NULL,
        [TaxAmount] DECIMAL(18,2) NOT NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [CouponCodeApplied] NVARCHAR(30) NULL,
        [Status] INT NOT NULL CONSTRAINT [DF_CustomerOrders_Status] DEFAULT (0),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_CustomerOrders_CreatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_CustomerOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_CustomerOrders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
    );
END
GO

-- 5. CustomerOrderItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOrderItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerOrderItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [ItemId] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [Quantity] INT NOT NULL,
        [TotalPrice] DECIMAL(18,2) NOT NULL,

        CONSTRAINT [PK_CustomerOrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_CustomerOrderItems_CustomerOrders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[CustomerOrders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CustomerOrderItems_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id])
    );
END
GO

PRINT 'OrderManagementDb setup complete.';
