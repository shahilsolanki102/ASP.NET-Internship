# ASP.NET Core Enterprise Internship & Engineering Portfolio

Welcome to the comprehensive 4-Week ASP.NET Core & DevOps Internship repository developed by **Shahil** (Yuva Intern / NSDC Track).

---

## 🌟 4-Week Curriculum & Deliverable Overview

| Milestone | Project Title | Key Technologies | Status | Links & Artifacts |
|---|---|---|---|---|
| **Week 1** | **ASP.NET Core SaaS User Profile Module** | ASP.NET Core 9.0 MVC, EF Core, SQL Server, Auth Cookies, SHA-256, 2FA, Dark/Light Mode, SweetAlert2 | ✅ Complete | [Explore Week 1](Week-1_UserProfileManagement/) &bull; [Report Docx](Week-1_UserProfileManagement/docs/Internship_Report_Week1.docx) |
| **Week 2** | **Code Debugging, Refactoring & Performance Tuning** | N+1 Query Resolution, B-Tree Indexes, IMemoryCache, AsNoTracking, Server-Side Pagination, Benchmark Engine | ✅ Complete | [Explore Week 2](Week-2/) &bull; [Report Docx](Week-2/docs/Performance_Optimization_Report_Week2.docx) |
| **Week 3** | **Automated Unit & Integration Testing Suite** | xUnit, Moq, FluentAssertions, WebApplicationFactory, 26/26 Tests Passed (100%), In-App Test Dashboard | ✅ Complete | [Explore Week 3](Week-3/) &bull; [Report Docx](Week-3/docs/Automated_Testing_Report_Week3.docx) |
| **Week 4** | **Documentation, Deployment & DevOps Integration** | GitHub Actions CI/CD, Multi-Stage Dockerfile, Docker Compose, Nginx Proxy, Automated PowerShell Deploy | ✅ Complete | [Explore Week 4](Week-4/) &bull; [Report Docx](Week-4/docs/DevOps_and_Deployment_Report_Week4.docx) |

---

## 🛠️ Global Technology Stack

- **Framework**: .NET 9.0 (ASP.NET Core 9.0 MVC)
- **Database**: Microsoft SQL Server 2022 (`localhost\SQLEXPRESS` & Docker Linux Container)
- **ORM & Data Access**: Entity Framework Core 9.0.2 with SQL Server & In-Memory Providers
- **Testing & Mocking**: xUnit 2.9.2, Moq 4.20.72, FluentAssertions 6.12.2, `Microsoft.AspNetCore.Mvc.Testing`
- **DevOps & Containerization**: GitHub Actions (.github/workflows/ci-cd.yml), Docker, Docker Compose, Nginx
- **Frontend & UI**: Bootstrap 5.3.3, Bootstrap Icons, Chart.js, SweetAlert2, Glassmorphism Modern Theme

---

## 🚀 Quick Start Guide

### 1. Automated Test Execution across Solutions
```bash
dotnet test "Week-3/OrderManagementSuite.sln"
```

### 2. Automated Production Build & Package
```powershell
powershell -ExecutionPolicy Bypass -File "Week-4\deployment\deploy.ps1"
```

### 3. Docker Compose Orchestration
```bash
cd "Week-4/deployment"
docker-compose up -d --build
```
