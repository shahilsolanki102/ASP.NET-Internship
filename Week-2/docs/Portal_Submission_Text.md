# Week 2: Portal Submission Text (248 Words)

> **Instructions for User:** Copy and paste the text below directly into the "Write description for the report" box on the internship portal.

---

During Week 2 of the ASP.NET internship, I focused on code debugging, structural refactoring, and database query optimization for an enterprise ASP.NET Core 9.0 MVC application. The core objective was to diagnose legacy performance bottlenecks and transform them into a high-throughput, low-latency production architecture.

Key inefficiencies resolved include the N+1 database query anti-pattern, where 201 iterative round-trips were consolidated into a single LINQ projection query, cutting latency by 95.7%. In the database layer on Microsoft SQL Server (12,000+ transactional records), I eliminated expensive full-table scans by authoring composite non-clustered covering B-Tree indexes on foreign keys and date ranges, accelerating sales aggregation queries from 1,250ms down to 14ms (98.8% speedup).

To optimize application memory and concurrency, I transitioned synchronous blocking calls to asynchronous non-blocking I/O (async/await with CancellationToken), bypassed EF Core Change Tracking using .AsNoTracking() on read-only endpoints, and enforced database-level server-side pagination (.Skip.Take). Furthermore, I integrated IMemoryCache with sliding expiration for high-frequency lookup data (dropping latency to <0.5ms) and established a Global Exception Handling Middleware for structured error logging.

An interactive Live Benchmark Dashboard with real-time Chart.js visual comparisons was authored to validate and showcase these measurable performance improvements. All optimized source code, SQL tuning scripts, and technical documentation have been committed to GitHub.
