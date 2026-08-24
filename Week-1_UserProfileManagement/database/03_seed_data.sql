-- ============================================================================
-- Script: 03_seed_data.sql
-- Description: Inserts sample demo users and profiles for testing.
-- Password for all seed users is 'Admin@123' / 'User@123' (hashed using SHA-256)
-- Author: Shahil (ASP.NET MVC Internship - Week 1)
-- ============================================================================

USE [UserProfileDb];
GO

-- Seed Admin User (Password: Admin@123 -> SHA256: 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@yuva.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt])
    VALUES ('admin', 'admin@yuva.com', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin', 1, SYSUTCDATETIME());

    DECLARE @AdminId INT = SCOPE_IDENTITY();

    INSERT INTO [dbo].[UserProfiles] (
        [UserId], [FullName], [PhoneNumber], [Bio], [ProfilePictureUrl],
        [DateOfBirth], [Gender], [Address], [City], [State], [Country], [PostalCode]
    )
    VALUES (
        @AdminId, 'System Administrator', '+91 9876543210',
        'ASP.NET Core Full Stack Lead Developer & System Administrator.',
        '/images/default-avatar.png', '1995-05-15', 'Male',
        '101 Tech Hub, Infocity', 'Gandhinagar', 'Gujarat', 'India', '382007'
    );
END
GO

-- Seed Demo Intern User (Password: User@123 -> SHA256: a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'shahil@intern.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt])
    VALUES ('shahil', 'shahil@intern.com', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'User', 1, SYSUTCDATETIME());

    DECLARE @ShahilId INT = SCOPE_IDENTITY();

    INSERT INTO [dbo].[UserProfiles] (
        [UserId], [FullName], [PhoneNumber], [Bio], [ProfilePictureUrl],
        [DateOfBirth], [Gender], [Address], [City], [State], [Country], [PostalCode]
    )
    VALUES (
        @ShahilId, 'Shahil Patel', '+91 9123456789',
        'ASP.NET MVC Intern enthusiastic about building scalable web applications and clean UI designs.',
        '/images/default-avatar.png', '2003-08-20', 'Male',
        '45 Green Park Avenue', 'Ahmedabad', 'Gujarat', 'India', '380015'
    );
END
GO

PRINT 'Sample seed data inserted successfully.';
GO
