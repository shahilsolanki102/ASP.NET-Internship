# Week 3: Implementing Unit and Integration Tests for ASP.NET Modules

An enterprise automated testing suite for an ASP.NET Core 9.0 MVC Order Management & Billing module, demonstrating **xUnit**, **Moq**, **FluentAssertions**, and **`Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory)** with 100% automated test execution.

---

## 🌟 Key Highlights & Test Statistics

- **Total Test Cases**: **26 Automated Tests (100% Pass Rate)**
- **Unit Tests (`OrderManagement.UnitTests`)**: 20 tests covering business rules, discounts, taxes, inventory validation, and controller actions.
- **Integration Tests (`OrderManagement.IntegrationTests`)**: 6 end-to-end tests validating HTTP routing, ModelState validation, AntiForgery tokens, and database persistence.
- **In-App Visual Test Dashboard**: Real-time interactive UI test execution matrix (`/TestDashboard`).

---

## 🛠️ Architecture & Tech Stack

- **Framework**: ASP.NET Core 9.0 MVC (.NET 9.0)
- **Testing Framework**: xUnit 2.9.2
- **Mocking**: Moq 4.20.72
- **Assertions**: FluentAssertions 6.12.2
- **Integration Testing Host**: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`)
- **ORM / Database**: Entity Framework Core 9.0.2 with SQL Server and In-Memory Test Provider

---

## 🚀 How to Run Tests

### 1. Run via Command-Line Interface (CLI)
```bash
cd "d:\ASP.NET Intern\Week-3"
dotnet test
```

### 2. Run via Visual Studio 2022 Test Explorer
1. Open `Week-3/OrderManagementSuite.sln` in Visual Studio 2022.
2. In the top menu, go to **Test &rarr; Test Explorer** (or press `Ctrl + E, T`).
3. Click **Run All Tests (▶️)** to view green checkmarks for all 26 tests.

### 3. Run Web Application
1. Set `OrderManagementApp` as the Startup Project.
2. Press **`Ctrl + F5`**.
3. Navigate to: `https://localhost:7xxx/TestDashboard` to explore the interactive visual test dashboard.

---

## 📂 Deliverables & Reports
- **Word Report**: `docs/Automated_Testing_Report_Week3.docx`
- **Portal Submission Description**: `docs/Portal_Submission_Text.md`
