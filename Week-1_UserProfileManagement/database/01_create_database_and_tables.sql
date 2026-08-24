-- ============================================================================
-- Script: 01_create_database_and_tables.sql
-- Description: Creates the UserProfileDb database, Users table, UserProfiles table,
--              constraints, foreign keys, and indexes for performance optimization.
-- Author: Shahil (ASP.NET MVC Internship - Week 1)
-- ============================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Create Database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UserProfileDb')
BEGIN
    CREATE DATABASE [UserProfileDb];
    PRINT 'Database UserProfileDb created successfully.';
END
GO

USE [UserProfileDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Create Users Table (Authentication & Account details)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Users_Role] DEFAULT ('User'),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Users_IsActive] DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [LastLoginAt] DATETIME2(7) NULL,

        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
    PRINT 'Table [dbo].[Users] created successfully.';
END
GO

-- Index on Users Email for fast authentication lookups
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = N'IX_Users_Email')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email] ASC);
    PRINT 'Index IX_Users_Email created.';
END
GO

-- 2. Create UserProfiles Table (Personal, Contact, and Demographic information)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserProfiles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [Bio] NVARCHAR(500) NULL,
        [ProfilePictureUrl] NVARCHAR(300) NULL,
        [DateOfBirth] DATE NULL,
        [Gender] NVARCHAR(20) NULL,
        [Address] NVARCHAR(250) NULL,
        [City] NVARCHAR(100) NULL,
        [State] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserProfiles_UpdatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_UserProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_UserProfiles_UserId] UNIQUE NONCLUSTERED ([UserId] ASC),
        CONSTRAINT [FK_UserProfiles_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE CASCADE
            ON UPDATE CASCADE
    );
    PRINT 'Table [dbo].[UserProfiles] created successfully.';
END
GO

-- Index on UserProfiles PhoneNumber for contact lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = N'IX_UserProfiles_PhoneNumber')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserProfiles_PhoneNumber] ON [dbo].[UserProfiles] ([PhoneNumber] ASC);
    PRINT 'Index IX_UserProfiles_PhoneNumber created.';
END
GO
