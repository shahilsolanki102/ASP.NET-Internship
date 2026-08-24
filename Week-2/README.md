# Week 2: Code Debugging, Refactoring, and Performance Optimization

An enterprise ASP.NET Core 9.0 MVC suite demonstrating real-world debugging, architectural refactoring, and database query optimization with a live interactive Benchmark Dashboard.

---

## 🌟 Key Highlights & Performance Gains

- **95.7% Faster Data Access**: Resolved the N+1 query problem, reducing 201 individual queries to 1 single projection query.
- **98.8% Faster SQL Aggregations**: Transitioned in-memory LINQ groupings to SQL Server engine execution (1,250ms &rarr; 14ms).
- **99.3% Faster Lookups**: Integrated `IMemoryCache` with sliding expiration (45.2ms &rarr; 0.3ms).
- **62% RAM Reduction**: Enforced `.AsNoTracking()` and database-level server-side pagination (`.Skip().Take()`).
- **Live Benchmark Dashboard**: Interactive Chart.js visual comparison of latency, queries, and memory.

---

## 🛠️ Architecture & Tech Stack

- **Framework**: ASP.NET Core 9.0 MVC (.NET 9.0)
- **Database**: Microsoft SQL Server 2022 (`localhost\SQLEXPRESS`) with 12,000+ benchmark records
- **ORM**: Entity Framework Core 9.0.2 with SQL Server Provider
- **Caching**: `Microsoft.Extensions.Caching.Memory` (`IMemoryCache`)
- **Frontend**: Bootstrap 5.3.3, Bootstrap Icons, Chart.js, Dark/Light Mode

---

## 🚀 How to Run the Application

### 1. Database Setup
Execute the scripts in order in SQL Server Management Studio or `sqlcmd`:
1. `database/01_create_database_and_tables.sql`
2. `database/02_performance_indexes_and_optimizations.sql`
3. `database/03_seed_benchmark_dataset.sql`

### 2. Run via Visual Studio 2022
1. Open `Week-2/PerformanceOptimization.sln` in Visual Studio 2022.
2. Set `PerformanceOptimizationApp` as the Startup Project.
3. Press **`F5`** or **`Ctrl + F5`**.

### 3. Run via Terminal
```bash
cd "d:\ASP.NET Intern\Week-2\src\PerformanceOptimizationApp"
dotnet run
```
Navigate to: `https://localhost:7xxx/Benchmark` to run live performance comparisons.

---

## 📂 Deliverables & Reports
- **Word Report**: `docs/Performance_Optimization_Report_Week2.docx`
- **Portal Submission Description**: `docs/Portal_Submission_Text.md`
- **Refactoring Guide**: `docs/Code_Refactoring_and_Debugging_Guide.md`
