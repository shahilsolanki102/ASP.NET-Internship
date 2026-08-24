# Week 3: Portal Submission Text (248 Words)

> **Instructions for User:** Copy and paste the text below directly into the "Write description for the report" box on the internship portal.

---

During Week 3 of the ASP.NET Core internship, I implemented a comprehensive Automated Unit and Integration Testing Suite for an enterprise Order Management and Billing module in ASP.NET Core 9.0 MVC. The core objective was to ensure software reliability, data integrity, and fault tolerance across critical business workflows, inventory rules, and financial calculations.

Using xUnit, Moq, and FluentAssertions, I designed 20 unit tests covering domain business logic: subtotal aggregations, 8% sales tax computations, customer tier discounts (VIP 10%, Enterprise 20%), coupon validation algorithms (expiration dates, minimum order thresholds, maximum discount caps), and edge-case guards (insufficient stock, non-positive quantities, and invalid item identifiers). Dependency injection dependencies like email notification dispatchers were isolated and verified using Mock invocation constraints (Times.Once).

For system-level verification, I authored 6 end-to-end integration tests using Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory) and an isolated test server host, verifying HTTP routing, Anti-Forgery token protection, ModelState validation, and full database persistence without external database dependencies.

All 26 automated tests execute seamlessly via the command-line interface (dotnet test) and Visual Studio Test Explorer with a 100% pass rate. Additionally, an in-app Visual Automated Test Dashboard was built to display real-time test execution matrices, assertion descriptions, and execution duration. Complete test suites, application source code, and technical documentation have been committed to GitHub.
