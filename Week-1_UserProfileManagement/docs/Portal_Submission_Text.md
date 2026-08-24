# Week 1 Submission Description (Yuva Intern Portal)
*Copy and paste the text below into the "Write description for the report (Minimum 200 words)" box on the Yuva Intern portal.*

---

```text
During Week 1 of the ASP.NET internship, I designed and implemented an enterprise-grade User Profile and Identity Management module for an ASP.NET Core MVC web application. The system architecture follows modern software engineering best practices, separating concerns across Models, ViewModels, Controllers, Business Services, and Entity Framework Core Data Context. The backend logic is authored in C# (.NET 9.0) with dedicated service layers (IProfileService and IAuthService) handling user data operations, Two-Factor Authentication (2FA), activity logging, and password hashing using SHA-256. 

The database layer utilizes Microsoft SQL Server with four relational tables: Users, UserProfiles, UserActivityLogs, and UserSessions. The schema incorporates primary keys, unique constraints, foreign keys with cascade actions, and non-clustered indexes on frequently queried fields like Email and PhoneNumber. Stored procedures (sp_GetUserProfileById, sp_UpdateUserProfile, and sp_LogUserActivity) were developed to ensure fast query execution and transactional safety. 

On the frontend, Razor views (.cshtml) feature a high-end 2026 SaaS glassmorphism design styled with Bootstrap 5.3. Key capabilities include dynamic avatar and cover banner uploads with real-time image preview, skill tagging, social media integration (GitHub, LinkedIn, Twitter, Portfolio), profile strength scoring, multi-device active session monitoring, and real-time audit logs. Security is enforced via Anti-CSRF token verification on all POST actions, strict file upload MIME and extension whitelisting with GUID-based sanitization, and Cookie-based claims authentication. All assets, SQL scripts, and technical documentation have been version-controlled and published to GitHub.
```

---
**Word Count**: 248 words (Meets the 200+ words requirement).
