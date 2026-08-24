-- ============================================================================
-- Script: 02_stored_procedures.sql
-- Description: Stored procedures for User Profile CRUD, Activity Logging, and Upserts.
-- Author: Shahil (ASP.NET Core MVC Enterprise Profile Module)
-- ============================================================================

USE [UserProfileDb];
GO

-- 1. Get Full User Profile Details
CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserProfileById]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.[Id] AS UserId,
        u.[Username],
        u.[Email],
        u.[Role],
        u.[IsActive],
        u.[TwoFactorEnabled],
        u.[CreatedAt] AS AccountCreatedAt,
        u.[LastLoginAt],
        u.[LastLoginIp],
        p.[Id] AS ProfileId,
        p.[FullName],
        p.[Headline],
        p.[PhoneNumber],
        p.[Bio],
        p.[ProfilePictureUrl],
        p.[CoverPhotoUrl],
        p.[DateOfBirth],
        p.[Gender],
        p.[Address],
        p.[City],
        p.[State],
        p.[Country],
        p.[PostalCode],
        p.[WebsiteUrl],
        p.[GitHubUrl],
        p.[LinkedInUrl],
        p.[TwitterUrl],
        p.[Skills],
        p.[TimeZone],
        p.[Language],
        p.[ProfileCompletionPercentage],
        p.[IsProfilePublic],
        p.[EmailNotifications],
        p.[UpdatedAt] AS ProfileUpdatedAt
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[UserProfiles] p ON u.[Id] = p.[UserId]
    WHERE u.[Id] = @UserId;
END
GO

-- 2. Log User Activity
CREATE OR ALTER PROCEDURE [dbo].[sp_LogUserActivity]
    @UserId INT,
    @ActivityType NVARCHAR(50),
    @Description NVARCHAR(250),
    @IpAddress NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[UserActivityLogs] ([UserId], [ActivityType], [Description], [IpAddress], [CreatedAt])
    VALUES (@UserId, @ActivityType, @Description, @IpAddress, SYSUTCDATETIME());
END
GO
