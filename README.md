# ASP.NET Web Development Internship Projects
**Platform**: Yuva Intern / NSDC  
**Intern**: Shahil  
**Technology Stack**: ASP.NET Core 9.0 MVC, Entity Framework Core 9, Microsoft SQL Server, Bootstrap 5  

---

## 📁 Repository Structure

```text
ASP.NET-Internship/
│
├── Week-1_UserProfileManagement/
│   ├── src/
│   │   └── UserProfileApp/               # ASP.NET Core 9.0 MVC Web Application
│   │       ├── Controllers/              # ProfileController, AccountController, HomeController
│   │       ├── Data/                     # ApplicationDbContext (EF Core)
│   │       ├── Models/                   # User, UserProfile
│   │       ├── ViewModels/               # DTOs & Form ViewModels
│   │       ├── Services/                 # ProfileService, AuthService, PasswordHelper
│   │       ├── Views/                    # Razor Views (Profile, Account, Home, Shared)
│   │       └── wwwroot/                  # Static assets & profile avatar uploads
│   ├── database/
│   │   ├── 01_create_database_and_tables.sql   # Database, Tables, Constraints & Indexes
│   │   ├── 02_stored_procedures.sql            # Stored Procedures for CRUD & Transactions
│   │   └── 03_seed_data.sql                    # Initial Demo Users & Profiles
│   └── docs/
│       ├── Design_Document_UserProfileModule.md # Comprehensive Technical Design Document
│       ├── Internship_Report_Week1.docx         # Formal Word Report (.docx)
│       └── Portal_Submission_Text.md            # 200+ Words Portal Description Text
│
├── Week-2/                               # Upcoming Week 2 Task
├── Week-3/                               # Upcoming Week 3 Task
└── Week-4/                               # Upcoming Week 4 Task
```

---

## 🚀 Week 1: User Profile Management Module

### ✨ Features Implemented
1. **User Profile View**: Clean, card-based interface displaying personal details, bio, contact information, address, and account timestamps.
2. **Profile Editing & Photo Upload**: Interactive form with live avatar preview, secure file uploading (JPG, PNG, WebP with size capping and GUID sanitization).
3. **Account Security**: SHA-256 password hashing and password change capability.
4. **Authentication & Authorization**: Cookie-based authentication with role claims and protected routes.
5. **Database Architecture**: SQL Server relational schema with primary/foreign keys, cascade actions, indexes on `Email` & `PhoneNumber`, and stored procedures.
6. **Responsive UI**: Built with Bootstrap 5 and Bootstrap Icons for seamless desktop and mobile viewports.

---

## 🛠️ Quick Start & Local Setup

### 1. Database Setup
Execute the SQL scripts in SQL Server Management Studio (SSMS) or via `sqlcmd`:
```bash
sqlcmd -S "localhost\SQLEXPRESS" -E -i "Week-1_UserProfileManagement\database\01_create_database_and_tables.sql"
sqlcmd -S "localhost\SQLEXPRESS" -E -i "Week-1_UserProfileManagement\database\02_stored_procedures.sql"
sqlcmd -S "localhost\SQLEXPRESS" -E -i "Week-1_UserProfileManagement\database\03_seed_data.sql"
```

### 2. Run ASP.NET Core Application
```bash
cd "Week-1_UserProfileManagement\src\UserProfileApp"
dotnet restore
dotnet run
```
Open your browser and navigate to: `https://localhost:5001` or `http://localhost:5000`

### 3. Demo Credentials
- **Intern Demo**:
  - Email: `shahil@intern.com`
  - Password: `User@123`
- **Admin Demo**:
  - Email: `admin@yuva.com`
  - Password: `Admin@123`
