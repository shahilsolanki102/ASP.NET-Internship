-- ============================================================================
-- Script: 01_create_database_and_tables.sql
-- Description: Creates UserProfileDb database with enterprise schema:
--              Users, UserProfiles, UserActivityLogs, and UserSessions.
-- Author: Shahil (ASP.NET Core MVC Enterprise Profile Module)
-- ============================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

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

-- 1. Users Table (Authentication, Security & Identity)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Users_Role] DEFAULT ('User'),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Users_IsActive] DEFAULT (1),
        [TwoFactorEnabled] BIT NOT NULL CONSTRAINT [DF_Users_TwoFactorEnabled] DEFAULT (0),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [LastLoginAt] DATETIME2(7) NULL,
        [LastLoginIp] NVARCHAR(50) NULL,

        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
    PRINT 'Table [dbo].[Users] created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = N'IX_Users_Email')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email] ASC);
    PRINT 'Index IX_Users_Email created.';
END
GO

-- 2. UserProfiles Table (Rich Profile Data, Socials, Preferences)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserProfiles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Headline] NVARCHAR(150) NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [Bio] NVARCHAR(1000) NULL,
        [ProfilePictureUrl] NVARCHAR(300) NULL,
        [CoverPhotoUrl] NVARCHAR(300) NULL,
        [DateOfBirth] DATE NULL,
        [Gender] NVARCHAR(20) NULL,
        [Address] NVARCHAR(250) NULL,
        [City] NVARCHAR(100) NULL,
        [State] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [WebsiteUrl] NVARCHAR(200) NULL,
        [GitHubUrl] NVARCHAR(200) NULL,
        [LinkedInUrl] NVARCHAR(200) NULL,
        [TwitterUrl] NVARCHAR(200) NULL,
        [Skills] NVARCHAR(500) NULL,
        [TimeZone] NVARCHAR(100) NULL CONSTRAINT [DF_UserProfiles_TimeZone] DEFAULT ('(GMT+05:30) India Standard Time'),
        [Language] NVARCHAR(50) NULL CONSTRAINT [DF_UserProfiles_Language] DEFAULT ('English (US)'),
        [ProfileCompletionPercentage] INT NOT NULL CONSTRAINT [DF_UserProfiles_Completion] DEFAULT (25),
        [IsProfilePublic] BIT NOT NULL CONSTRAINT [DF_UserProfiles_IsPublic] DEFAULT (1),
        [EmailNotifications] BIT NOT NULL CONSTRAINT [DF_UserProfiles_EmailNotif] DEFAULT (1),
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
ELSE
BEGIN
    -- Add columns if upgrading existing table
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'Headline')
        ALTER TABLE [dbo].[UserProfiles] ADD [Headline] NVARCHAR(150) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'CoverPhotoUrl')
        ALTER TABLE [dbo].[UserProfiles] ADD [CoverPhotoUrl] NVARCHAR(300) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'WebsiteUrl')
        ALTER TABLE [dbo].[UserProfiles] ADD [WebsiteUrl] NVARCHAR(200) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'GitHubUrl')
        ALTER TABLE [dbo].[UserProfiles] ADD [GitHubUrl] NVARCHAR(200) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'LinkedInUrl')
        ALTER TABLE [dbo].[UserProfiles] ADD [LinkedInUrl] NVARCHAR(200) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'TwitterUrl')
        ALTER TABLE [dbo].[UserProfiles] ADD [TwitterUrl] NVARCHAR(200) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'Skills')
        ALTER TABLE [dbo].[UserProfiles] ADD [Skills] NVARCHAR(500) NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'TimeZone')
        ALTER TABLE [dbo].[UserProfiles] ADD [TimeZone] NVARCHAR(100) NULL DEFAULT ('(GMT+05:30) India Standard Time');
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'Language')
        ALTER TABLE [dbo].[UserProfiles] ADD [Language] NVARCHAR(50) NULL DEFAULT ('English (US)');
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'ProfileCompletionPercentage')
        ALTER TABLE [dbo].[UserProfiles] ADD [ProfileCompletionPercentage] INT NOT NULL DEFAULT (25);
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'IsProfilePublic')
        ALTER TABLE [dbo].[UserProfiles] ADD [IsProfilePublic] BIT NOT NULL DEFAULT (1);
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = 'EmailNotifications')
        ALTER TABLE [dbo].[UserProfiles] ADD [EmailNotifications] BIT NOT NULL DEFAULT (1);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = N'IX_UserProfiles_PhoneNumber')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserProfiles_PhoneNumber] ON [dbo].[UserProfiles] ([PhoneNumber] ASC);
    PRINT 'Index IX_UserProfiles_PhoneNumber created.';
END
GO

-- 3. UserActivityLogs Table (Audit Trail & Activity Timeline)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserActivityLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserActivityLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [ActivityType] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(250) NOT NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserActivityLogs_CreatedAt] DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT [PK_UserActivityLogs] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserActivityLogs_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE CASCADE
    );
    PRINT 'Table [dbo].[UserActivityLogs] created successfully.';
END
GO

-- 4. UserSessions Table (Active Security Devices / Sessions)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSessions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserSessions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Device] NVARCHAR(100) NOT NULL,
        [Browser] NVARCHAR(100) NOT NULL,
        [IpAddress] NVARCHAR(50) NOT NULL,
        [Location] NVARCHAR(100) NULL,
        [LastActive] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserSessions_LastActive] DEFAULT (SYSUTCDATETIME()),
        [IsCurrent] BIT NOT NULL CONSTRAINT [DF_UserSessions_IsCurrent] DEFAULT (0),

        CONSTRAINT [PK_UserSessions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserSessions_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE CASCADE
    );
    PRINT 'Table [dbo].[UserSessions] created successfully.';
END
GO
