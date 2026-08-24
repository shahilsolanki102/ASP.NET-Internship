-- ============================================================================
-- Script: 03_seed_data.sql
-- Description: Realistic production-like seed data with activities and sessions.
-- Passwords:
--   shahil@intern.com -> User@123  (SHA256: 3e7c19576488862816f13b512cacf3e4ba97dd97243ea0bd6a2ad1642d86ba72)
--   admin@yuva.com    -> Admin@123 (SHA256: e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7)
-- Author: Shahil (ASP.NET Core MVC Enterprise Profile Module)
-- ============================================================================

USE [UserProfileDb];
GO

-- 1. Seed Intern Profile (Shahil Patel)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'shahil@intern.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [IsActive], [TwoFactorEnabled], [CreatedAt], [LastLoginAt], [LastLoginIp])
    VALUES ('shahil', 'shahil@intern.com', '3e7c19576488862816f13b512cacf3e4ba97dd97243ea0bd6a2ad1642d86ba72', 'User', 1, 1, DATEADD(DAY, -15, SYSUTCDATETIME()), SYSUTCDATETIME(), '192.168.1.105');

    DECLARE @ShahilId INT = SCOPE_IDENTITY();

    INSERT INTO [dbo].[UserProfiles] (
        [UserId], [FullName], [Headline], [PhoneNumber], [Bio], [ProfilePictureUrl], [CoverPhotoUrl],
        [DateOfBirth], [Gender], [Address], [City], [State], [Country], [PostalCode],
        [WebsiteUrl], [GitHubUrl], [LinkedInUrl], [TwitterUrl], [Skills], [TimeZone], [Language],
        [ProfileCompletionPercentage], [IsProfilePublic], [EmailNotifications], [UpdatedAt]
    )
    VALUES (
        @ShahilId, 'Shahil Patel', 'ASP.NET Core & Full-Stack Cloud Engineer | Passionate Problem Solver',
        '+91 98765 43210',
        'Aspiring software engineer specialized in building resilient web applications with ASP.NET Core, C#, Entity Framework Core, and SQL Server. Constantly exploring modern UI designs, microservices architectures, and cloud deployments.',
        'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=400&auto=format&fit=crop&q=80',
        'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1200&auto=format&fit=crop&q=80',
        '2003-08-20', 'Male', '402 Sunrise Heights, S.G. Highway', 'Ahmedabad', 'Gujarat', 'India', '380054',
        'https://shahil.dev', 'https://github.com/shahil-dev', 'https://linkedin.com/in/shahil-patel', 'https://twitter.com/shahil_dev',
        'C#, ASP.NET Core MVC, Entity Framework Core, SQL Server, REST APIs, Bootstrap 5, JavaScript, Docker, Git',
        '(GMT+05:30) India Standard Time', 'English (US)', 90, 1, 1, SYSUTCDATETIME()
    );

    -- Seed Activity Logs
    INSERT INTO [dbo].[UserActivityLogs] ([UserId], [ActivityType], [Description], [IpAddress], [CreatedAt])
    VALUES 
        (@ShahilId, 'Security', 'Enabled Two-Factor Authentication (2FA)', '192.168.1.105', DATEADD(HOUR, -2, SYSUTCDATETIME())),
        (@ShahilId, 'Profile', 'Updated Bio and Social Media Handles (GitHub & LinkedIn)', '192.168.1.105', DATEADD(DAY, -1, SYSUTCDATETIME())),
        (@ShahilId, 'Avatar', 'Changed Profile Picture and Cover Banner', '192.168.1.105', DATEADD(DAY, -3, SYSUTCDATETIME())),
        (@ShahilId, 'Account', 'Account successfully created and verified', '192.168.1.105', DATEADD(DAY, -15, SYSUTCDATETIME()));

    -- Seed Active Sessions
    INSERT INTO [dbo].[UserSessions] ([UserId], [Device], [Browser], [IpAddress], [Location], [LastActive], [IsCurrent])
    VALUES 
        (@ShahilId, 'Windows PC (Desktop)', 'Chrome 128.0 (Windows 11)', '192.168.1.105', 'Ahmedabad, India', SYSUTCDATETIME(), 1),
        (@ShahilId, 'iPhone 15 Pro (Mobile)', 'Safari 18.0 (iOS)', '103.212.144.12', 'Ahmedabad, India', DATEADD(HOUR, -5, SYSUTCDATETIME()), 0),
        (@ShahilId, 'MacBook Air M2 (Laptop)', 'Brave Browser', '103.212.144.18', 'Gandhinagar, India', DATEADD(DAY, -2, SYSUTCDATETIME()), 0);
END
ELSE
BEGIN
    UPDATE [dbo].[Users]
    SET [PasswordHash] = '3e7c19576488862816f13b512cacf3e4ba97dd97243ea0bd6a2ad1642d86ba72'
    WHERE [Email] = 'shahil@intern.com';
END
GO

-- 2. Seed Admin Profile
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@yuva.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [IsActive], [TwoFactorEnabled], [CreatedAt], [LastLoginAt], [LastLoginIp])
    VALUES ('admin', 'admin@yuva.com', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', 'Admin', 1, 1, DATEADD(DAY, -60, SYSUTCDATETIME()), SYSUTCDATETIME(), '127.0.0.1');

    DECLARE @AdminId INT = SCOPE_IDENTITY();

    INSERT INTO [dbo].[UserProfiles] (
        [UserId], [FullName], [Headline], [PhoneNumber], [Bio], [ProfilePictureUrl], [CoverPhotoUrl],
        [DateOfBirth], [Gender], [Address], [City], [State], [Country], [PostalCode],
        [WebsiteUrl], [GitHubUrl], [LinkedInUrl], [TwitterUrl], [Skills], [TimeZone], [Language],
        [ProfileCompletionPercentage], [IsProfilePublic], [EmailNotifications], [UpdatedAt]
    )
    VALUES (
        @AdminId, 'System Administrator', 'Lead Enterprise Solutions Architect & System Admin',
        '+91 99999 88888',
        'Managing organizational infrastructure, database security, and microservices lifecycle.',
        'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=400&auto=format&fit=crop&q=80',
        'https://images.unsplash.com/photo-1579546929518-9e396f3cc809?w=1200&auto=format&fit=crop&q=80',
        '1995-03-12', 'Male', '100 Cyber City Boulevard', 'Gandhinagar', 'Gujarat', 'India', '382007',
        'https://admin.yuva.com', 'https://github.com/admin-yuva', 'https://linkedin.com/in/admin-yuva', 'https://twitter.com/admin_yuva',
        'System Architecture, Cloud Security, DevOps, SQL Tuning, .NET Core, Microservices',
        '(GMT+05:30) India Standard Time', 'English (US)', 100, 1, 1, SYSUTCDATETIME()
    );
END
ELSE
BEGIN
    UPDATE [dbo].[Users]
    SET [PasswordHash] = 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7'
    WHERE [Email] = 'admin@yuva.com';
END
GO
