# Technical Design Document: User Profile Management Module
**Project Title**: ASP.NET Core MVC User Profile Management Feature  
**Internship Week**: Week 1 (Yuva Intern / NSDC)  
**Author**: Shahil  
**Framework**: ASP.NET Core 9.0 MVC (C#)  
**Database**: Microsoft SQL Server (SQLEXPRESS / LocalDB)  

---

## 1. Executive Summary & Objective
The objective of this project is to architect, develop, and deliver a secure, robust, and responsive **User Profile Management Module** within an ASP.NET Core MVC web application. The module empowers users to manage their identity, personal bio, contact numbers, residential address, profile avatar, and account security credentials seamlessly.

---

## 2. System Architecture

The application adopts the **Model-View-Controller (MVC)** architectural design pattern, supplemented with a **Service/Data Access Layer** for clean separation of concerns and maintainability:

```
┌────────────────────────────────────────────────────────┐
│                   Presentation Layer                   │
│        Razor Views (.cshtml) + Bootstrap 5 + JS        │
└───────────────────────────┬────────────────────────────┘
                            │ HTTP Requests / Form Posts
┌───────────────────────────▼────────────────────────────┐
│                    Controller Layer                    │
│   • ProfileController      • AccountController         │
│   • HomeController                                     │
└───────────────────────────┬────────────────────────────┘
                            │ ViewModels / DTOs
┌───────────────────────────▼────────────────────────────┐
│                  Business Service Layer                │
│   • IProfileService / ProfileService                   │
│   • IAuthService / AuthService                         │
│   • PasswordHelper (SHA-256 Hashing)                   │
└───────────────────────────┬────────────────────────────┘
                            │ Entity Models
┌───────────────────────────▼────────────────────────────┐
│                    Data Access Layer                   │
│   • Entity Framework Core 9 (ApplicationDbContext)     │
│   • Microsoft SQL Server Database (UserProfileDb)      │
│   • Stored Procedures (`sp_GetUserProfileById`, etc.)  │
└────────────────────────────────────────────────────────┘
```

---

## 3. Database Schema & Data Dictionary

### 3.1 `dbo.Users` Table
Stores account authentication and security identity data.

| Column Name | Data Type | Nullable | Description / Constraint |
|---|---|---|---|
| `Id` | `INT` | No | Primary Key, Identity(1,1) |
| `Username` | `NVARCHAR(50)` | No | Unique constraint (`UQ_Users_Username`) |
| `Email` | `NVARCHAR(150)` | No | Unique constraint (`UQ_Users_Email`), Indexed |
| `PasswordHash` | `NVARCHAR(256)` | No | SHA-256 cryptographic password hash |
| `Role` | `NVARCHAR(20)` | No | Default: `'User'` |
| `IsActive` | `BIT` | No | Default: `1` (Active status) |
| `CreatedAt` | `DATETIME2(7)` | No | Default: `SYSUTCDATETIME()` |
| `LastLoginAt` | `DATETIME2(7)` | Yes | Updated upon successful authentication |

### 3.2 `dbo.UserProfiles` Table
Stores personal details, profile picture URLs, address, and metadata.

| Column Name | Data Type | Nullable | Description / Constraint |
|---|---|---|---|
| `Id` | `INT` | No | Primary Key, Identity(1,1) |
| `UserId` | `INT` | No | Foreign Key -> `Users(Id)` (ON DELETE CASCADE), Unique |
| `FullName` | `NVARCHAR(100)` | No | User's full display name |
| `PhoneNumber` | `NVARCHAR(20)` | Yes | Contact number, Indexed (`IX_UserProfiles_PhoneNumber`) |
| `Bio` | `NVARCHAR(500)` | Yes | Personal biography or professional summary |
| `ProfilePictureUrl` | `NVARCHAR(300)` | Yes | Web relative path to uploaded avatar image |
| `DateOfBirth` | `DATE` | Yes | User's birthdate |
| `Gender` | `NVARCHAR(20)` | Yes | Gender identity |
| `Address` | `NVARCHAR(250)` | Yes | Street address |
| `City` | `NVARCHAR(100)` | Yes | City name |
| `State` | `NVARCHAR(100)` | Yes | State / Province |
| `Country` | `NVARCHAR(100)` | Yes | Country name |
| `PostalCode` | `NVARCHAR(20)` | Yes | Postal / Zip code |
| `UpdatedAt` | `DATETIME2(7)` | No | Timestamp of latest profile modification |

---

## 4. Stored Procedures Specification
1. **`sp_GetUserProfileById`**: Retrieves combined user authentication and profile demographic records using an optimized `LEFT JOIN`.
2. **`sp_UpdateUserProfile`**: Performs upsert logic (updating existing profile or creating a new record if missing) with auto-updating timestamps.
3. **`sp_RegisterUserWithProfile`**: Atomic transaction ensuring both user account and initial profile record are created simultaneously.

---

## 5. Security & Validation Measures

1. **Password Hashing**: Implements SHA-256 secure cryptographic hashing with salt-compatible structures to ensure plain-text passwords are never stored.
2. **Anti-Forgery Token Verification**: All state-modifying POST requests (`Edit`, `ChangePassword`, `Login`, `Register`, `Logout`) are guarded by `[ValidateAntiForgeryToken]` to eliminate Cross-Site Request Forgery (CSRF).
3. **Secure File Upload**:
   - MIME/extension whitelist (`.jpg`, `.jpeg`, `.png`, `.webp`).
   - File size restriction (capped at 2 MB).
   - Server-side filename sanitization with globally unique identifiers (`Guid.NewGuid()`) to eliminate path traversal and file overwrite vulnerabilities.
4. **Input Validation**: Both client-side (jQuery Validation / unobtrusive) and server-side Model State validations (`[Required]`, `[EmailAddress]`, `[Phone]`, `[StringLength]`).
5. **Authorization Guards**: `[Authorize]` attributes prevent unauthenticated users from querying or editing profile endpoints.

---

## 6. Challenges Encountered & Solutions

| # | Challenge | Resolution |
|---|---|---|
| 1 | File upload naming collisions and security risks. | Implemented GUID-based unique file naming and strict extension whitelisting on the server. |
| 2 | Immediate synchronization of user avatar in navigation bar upon update. | Reissued Cookie Claims Principal (`SignInAsync`) with updated claims immediately after successful profile updates. |
| 3 | Filtered SQL Server indexes requiring ANSI settings. | Explicitly prepended `SET ANSI_NULLS ON` and `SET QUOTED_IDENTIFIER ON` across database creation scripts. |

---

## 7. Conclusion
The Week 1 ASP.NET MVC User Profile Management module fulfills all requirements specified in the Yuva Intern curriculum. The module is fully functional, follows clean coding principles, and provides an intuitive, responsive user experience.
