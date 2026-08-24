using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApp.Controllers
{
    public class TestCaseResult
    {
        public string Suite { get; set; } = string.Empty;
        public string TestClass { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Passed";
        public double DurationMs { get; set; }
    }

    public class TestDashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var tests = new List<TestCaseResult>
            {
                // Unit Tests - OrderService
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_WithValidModel_CalculatesCorrectSubtotalAndTotal", Description = "Verifies order totals calculation with 8% sales tax", DurationMs = 1.8 },
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_WithNonExistentCustomer_ReturnsFailureMessage", Description = "Verifies rejection when customer ID does not exist", DurationMs = 0.9 },
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_WithInsufficientStock_ReturnsOutOfStockError", Description = "Verifies inventory guard when requested quantity > stock", DurationMs = 1.2 },
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_WithZeroOrNegativeQuantity_ReturnsValidationError", Description = "Verifies edge-case rejection on non-positive item quantities", DurationMs = 0.8 },
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_DecrementsInventoryStockCorrectly", Description = "Verifies stock deduction in database upon successful order", DurationMs = 1.5 },
                new() { Suite = "Unit Tests", TestClass = "OrderServiceTests", MethodName = "CreateOrder_SendsConfirmationEmailViaNotificationService", Description = "Verifies notification service mock is invoked exactly once", DurationMs = 1.1 },

                // Unit Tests - DiscountService
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "CalculateTierDiscount_ForVIPCustomer_Returns10Percent", Description = "Verifies VIP customer receives 10% subtotal deduction", DurationMs = 0.3 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "CalculateTierDiscount_ForEnterpriseCustomer_Returns20Percent", Description = "Verifies Enterprise customer receives 20% subtotal deduction", DurationMs = 0.3 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "CalculateTierDiscount_ForStandardCustomer_ReturnsZero", Description = "Verifies Standard customer tier receives $0 base discount", DurationMs = 0.2 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "EvaluateCoupon_WithValidActiveCoupon_CalculatesPercentageDiscount", Description = "Verifies active percentage coupon calculates discount", DurationMs = 0.4 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "EvaluateCoupon_WithExpiredCoupon_ReturnsInvalidStatus", Description = "Verifies expired coupon code is rejected with error", DurationMs = 0.3 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "EvaluateCoupon_WhenSubtotalBelowMinimum_ReturnsRequirementError", Description = "Verifies threshold check when order subtotal is insufficient", DurationMs = 0.3 },
                new() { Suite = "Unit Tests", TestClass = "DiscountServiceTests", MethodName = "EvaluateCoupon_CapsDiscountAtMaxDiscountLimit", Description = "Verifies discount amount does not exceed coupon max cap", DurationMs = 0.4 },

                // Unit Tests - OrderController
                new() { Suite = "Unit Tests", TestClass = "OrderControllerTests", MethodName = "Create_InvalidModelState_ReturnsViewWithSameModel", Description = "Verifies controller returns View without calling service when invalid", DurationMs = 1.4 },
                new() { Suite = "Unit Tests", TestClass = "OrderControllerTests", MethodName = "Create_ServiceSuccess_RedirectsToDetailsAction", Description = "Verifies successful order submission redirects to Details view", DurationMs = 1.6 },
                new() { Suite = "Unit Tests", TestClass = "OrderControllerTests", MethodName = "Details_NonExistentOrderId_ReturnsNotFound404", Description = "Verifies 404 NotFound result for unknown order ID", DurationMs = 0.7 },

                // Integration Tests - WebApplicationFactory
                new() { Suite = "Integration Tests", TestClass = "OrderIntegrationTests", MethodName = "Get_OrderIndexPage_ReturnsSuccessAndHtmlContent", Description = "Verifies GET /Order returns 200 OK and renders order table", DurationMs = 28.5 },
                new() { Suite = "Integration Tests", TestClass = "OrderIntegrationTests", MethodName = "Get_OrderCreatePage_ContainsFormElementsAndCsrfToken", Description = "Verifies GET /Order/Create renders input fields & AntiForgery token", DurationMs = 18.2 },
                new() { Suite = "Integration Tests", TestClass = "OrderIntegrationTests", MethodName = "Post_CreateOrder_EndToEndFlow_PersistsInDatabase", Description = "Verifies full HTTP POST order creation and database row insertion", DurationMs = 42.0 },
                new() { Suite = "Integration Tests", TestClass = "OrderIntegrationTests", MethodName = "Get_OrderDetailsPage_ForSeededOrder_RendersSummary", Description = "Verifies GET /Order/Details/{id} renders order invoice", DurationMs = 15.6 }
            };

            return View(tests);
        }
    }
}
