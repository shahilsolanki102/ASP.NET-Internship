-- ============================================================================
-- Script: 02_stored_procedures.sql
-- Description: Creates stored procedures for user profile CRUD operations.
-- Author: Shahil (ASP.NET MVC Internship - Week 1)
-- ============================================================================

USE [UserProfileDb];
GO

-- 1. Stored Procedure: Get Complete User Profile by User ID
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
        u.[CreatedAt] AS AccountCreatedAt,
        u.[LastLoginAt],
        p.[Id] AS ProfileId,
        p.[FullName],
        p.[PhoneNumber],
        p.[Bio],
        p.[ProfilePictureUrl],
        p.[DateOfBirth],
        p.[Gender],
        p.[Address],
        p.[City],
        p.[State],
        p.[Country],
        p.[PostalCode],
        p.[UpdatedAt] AS ProfileUpdatedAt
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[UserProfiles] p ON u.[Id] = p.[UserId]
    WHERE u.[Id] = @UserId;
END
GO

-- 2. Stored Procedure: Update User Profile Details
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateUserProfile]
    @UserId INT,
    @FullName NVARCHAR(100),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Bio NVARCHAR(500) = NULL,
    @ProfilePictureUrl NVARCHAR(300) = NULL,
    @DateOfBirth DATE = NULL,
    @Gender NVARCHAR(20) = NULL,
    @Address NVARCHAR(250) = NULL,
    @City NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @Country NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if profile exists; if exists update, otherwise insert
    IF EXISTS (SELECT 1 FROM [dbo].[UserProfiles] WHERE [UserId] = @UserId)
    BEGIN
        UPDATE [dbo].[UserProfiles]
        SET 
            [FullName] = @FullName,
            [PhoneNumber] = @PhoneNumber,
            [Bio] = @Bio,
            [ProfilePictureUrl] = COALESCE(@ProfilePictureUrl, [ProfilePictureUrl]),
            [DateOfBirth] = @DateOfBirth,
            [Gender] = @Gender,
            [Address] = @Address,
            [City] = @City,
            [State] = @State,
            [Country] = @Country,
            [PostalCode] = @PostalCode,
            [UpdatedAt] = SYSUTCDATETIME()
        WHERE [UserId] = @UserId;
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[UserProfiles] (
            [UserId], [FullName], [PhoneNumber], [Bio], [ProfilePictureUrl],
            [DateOfBirth], [Gender], [Address], [City], [State], [Country], [PostalCode], [UpdatedAt]
        )
        VALUES (
            @UserId, @FullName, @PhoneNumber, @Bio, @ProfilePictureUrl,
            @DateOfBirth, @Gender, @Address, @City, @State, @Country, @PostalCode, SYSUTCDATETIME()
        );
    END

    -- Return updated profile
    EXEC [dbo].[sp_GetUserProfileById] @UserId = @UserId;
END
GO

-- 3. Stored Procedure: Register New User with Default Profile
CREATE OR ALTER PROCEDURE [dbo].[sp_RegisterUserWithProfile]
    @Username NVARCHAR(50),
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(256),
    @FullName NVARCHAR(100),
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Check if user already exists
        IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = @Username OR [Email] = @Email)
        BEGIN
            RAISERROR ('Username or Email already registered.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insert into Users
        INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [IsActive], [CreatedAt])
        VALUES (@Username, @Email, @PasswordHash, 'User', 1, SYSUTCDATETIME());

        SET @NewUserId = SCOPE_IDENTITY();

        -- Insert default UserProfile
        INSERT INTO [dbo].[UserProfiles] ([UserId], [FullName], [ProfilePictureUrl], [UpdatedAt])
        VALUES (@NewUserId, @FullName, '/images/default-avatar.png', SYSUTCDATETIME());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO
