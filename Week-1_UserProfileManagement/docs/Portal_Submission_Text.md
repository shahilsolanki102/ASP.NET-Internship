# Week 1 Submission Description (Yuva Intern Portal)
*Copy and paste the text below into the "Write description for the report (Minimum 200 words)" box on the Yuva Intern portal.*

---

```text
During Week 1 of the ASP.NET internship, I designed and implemented a full-stack User Profile Management module for an ASP.NET Core MVC web application. The core objective of this project was to provide users with an intuitive, secure, and responsive platform to view and manage their personal identity, contact details, bio, address information, profile avatar, and account security credentials.

The system architecture is structured according to enterprise MVC best practices, separating concerns across Models, ViewModels, Controllers, Business Services, and Entity Framework Core Data Context. The backend logic is written in C# (.NET 9.0) with custom service abstractions (IProfileService and IAuthService) handling user data operations, business rules, and password hashing using SHA-256. The database layer utilizes Microsoft SQL Server with two primary tables: Users and UserProfiles. The tables feature primary keys, unique constraints, foreign keys with cascade actions, and non-clustered indexes on high-frequency search fields such as Email and PhoneNumber for optimal query performance. Additionally, stored procedures (sp_GetUserProfileById, sp_UpdateUserProfile, and sp_RegisterUserWithProfile) were authored to support transactional data integrity.

On the frontend, Razor views (.cshtml) were styled using Bootstrap 5 to deliver a modern, clean, and mobile-responsive user interface. Key user-facing features include real-time image preview during avatar upload, tabbed profile cards, feedback toast alerts, and comprehensive client-side and server-side form validations. Security considerations were prioritized by implementing Anti-Forgery Token (CSRF) verification on all POST actions, strict file upload MIME and extension whitelisting (JPG, PNG, WebP) with size capping at 2MB, GUID-based safe filename sanitization, and Cookie-based authentication with role claims. The project has been thoroughly tested, documented, and published to GitHub with complete SQL schema scripts and technical design documentation.
```

---
**Word Count**: 267 words (Meets the 200+ words requirement).
