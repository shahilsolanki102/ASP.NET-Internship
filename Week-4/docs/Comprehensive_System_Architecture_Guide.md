# Comprehensive System Architecture Guide

## 🌐 4-Week Internship Architecture Overview

This document provides a holistic architectural blueprint of the enterprise ASP.NET Core 9.0 applications developed throughout the 4-week internship.

---

## 🏛️ High-Level Multi-Tier Architecture

```
+-------------------------------------------------------------------------------+
|                             Client Layer (Browser / UI)                       |
|           Bootstrap 5.3.3 | Glassmorphism Theme | Dark/Light Mode Switcher    |
+---------------------------------------+---------------------------------------+
                                        |
                                        v
+-------------------------------------------------------------------------------+
|                       Ingress & Security Layer (DevOps)                       |
|              Nginx Reverse Proxy | SSL Termination | Security Headers         |
+---------------------------------------+---------------------------------------+
                                        |
                                        v
+-------------------------------------------------------------------------------+
|                     ASP.NET Core 9.0 MVC Core Runtime Engine                  |
|  +---------------------------+  +------------------------------------------+  |
|  |       Controllers         |  |             Middleware Pipeline          |  |
|  | Profile, Benchmark, Order |  | Global Exception, AntiForgery, Auth      |  |
|  +-------------+-------------+  +--------------------+---------------------+  |
|                |                                     |                        |
|                v                                     v                        |
|  +---------------------------+  +------------------------------------------+  |
|  |    Business Services      |  |             Caching & Telemetry          |  |
|  | OrderService, ProfileSvc  |  | IMemoryCache (Sliding), Serilog Logging  |  |
|  +-------------+-------------+  +------------------------------------------+  |
+----------------|--------------------------------------------------------------+
                 |
                 v
+-------------------------------------------------------------------------------+
|                         Data Access Layer (EF Core 9.0)                       |
|        LINQ Projections (.Select) | AsNoTracking() | Compiled Stored Procs    |
+---------------------------------------+---------------------------------------+
                                        |
                                        v
+-------------------------------------------------------------------------------+
|                        Relational Database Engine                             |
|          Microsoft SQL Server 2022 | Covering B-Tree Indexes (NOLOCK)         |
+-------------------------------------------------------------------------------+
```

---

## 📦 Weekly Modules & Feature Decomposition

### Week 1: SaaS User Profile Management Module
- **Location**: `Week-1_UserProfileManagement/`
- **Core Focus**: Authentication & Authorization, Profile Details, Avatar/Cover uploads, Two-Factor Authentication (2FA) toggling, Activity Audit Logs, and Active Sessions.
- **Key Tech**: Cookie Authentication, SHA-256 Hashing, Glassmorphism UI, SweetAlert2.

### Week 2: Code Debugging, Refactoring & Performance Tuning
- **Location**: `Week-2/`
- **Core Focus**: Diagnosis and resolution of N+1 database queries, B-Tree covering index tuning, `IMemoryCache` lookup caching, and `.AsNoTracking()` memory optimization.
- **Benchmark Highlights**: 95.7% latency reduction on data access, 98.8% speedup on sales aggregations (1,250ms &rarr; 14ms).

### Week 3: Automated Unit & Integration Testing Suite
- **Location**: `Week-3/`
- **Core Focus**: xUnit, Moq, FluentAssertions, and `WebApplicationFactory<Program>` in-memory HTTP integration testing.
- **Coverage**: 26 automated tests passing with 100% success rate across domain rules, discounts, and database persistence.

### Week 4: Project Documentation, Deployment & DevOps Integration
- **Location**: `Week-4/` & `.github/workflows/`
- **Core Focus**: Multi-stage Docker containerization, `docker-compose.yml` orchestration, GitHub Actions CI/CD automation, and one-click deployment scripts.
