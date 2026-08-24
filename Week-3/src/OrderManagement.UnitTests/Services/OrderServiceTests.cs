using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagementApp.Data;
using OrderManagementApp.Models;
using OrderManagementApp.Services;
using OrderManagementApp.ViewModels;
using Xunit;

namespace OrderManagement.UnitTests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly OrderDbContext _context;
        private readonly DiscountService _discountService;
        private readonly Mock<INotificationService> _notificationMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _discountService = new DiscountService();
            _notificationMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            // Setup notification mock default
            _notificationMock
                .Setup(n => n.SendOrderConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(true);

            _orderService = new OrderService(_context, _discountService, _notificationMock.Object, _loggerMock.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Customers.AddRange(
                new Customer { Id = 1, Name = "Alice VIP", Email = "alice@vip.com", Tier = CustomerTier.VIP },
                new Customer { Id = 2, Name = "Bob Standard", Email = "bob@standard.com", Tier = CustomerTier.Standard }
            );

            _context.Items.AddRange(
                new Item { Id = 1, SKU = "LAPTOP-01", Name = "Dell Precision Laptop", UnitPrice = 1000.00m, StockQuantity = 10, IsActive = true },
                new Item { Id = 2, SKU = "MOUSE-01", Name = "Wireless Mouse", UnitPrice = 50.00m, StockQuantity = 5, IsActive = true },
                new Item { Id = 3, SKU = "INACTIVE-01", Name = "Discontinued Item", UnitPrice = 20.00m, StockQuantity = 10, IsActive = false }
            );

            _context.Coupons.Add(
                new Coupon { Id = 1, Code = "SAVE10", DiscountPercentage = 10, MinOrderAmount = 100, MaxDiscountAmount = 100, ExpiryDate = DateTime.UtcNow.AddDays(30), IsActive = true }
            );

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateOrderAsync_WithValidModel_CalculatesCorrectSubtotalTaxesAndTotal()
        {
            // Arrange (Alice VIP buys 1 laptop @ $1000 with SAVE10 coupon)
            // Subtotal = $1000
            // Tier Discount (VIP 10%) = $100
            // Coupon Discount (SAVE10 10%) = $100
            // Total Discount = $200
            // Taxable = $800
            // Tax (8%) = $64
            // Total = $864
            var model = new CreateOrderViewModel
            {
                CustomerId = 1,
                CouponCode = "SAVE10",
                Items = new List<CreateOrderItemInput>
                {
                    new CreateOrderItemInput { ItemId = 1, Quantity = 1 }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeTrue();
            result.Order.Should().NotBeNull();
            result.Order!.Subtotal.Should().Be(1000.00m);
            result.Order.DiscountAmount.Should().Be(200.00m);
            result.Order.TaxAmount.Should().Be(64.00m);
            result.Order.TotalAmount.Should().Be(864.00m);
            result.Order.Status.Should().Be(OrderStatus.Completed);

            // Verify notification email was sent
            _notificationMock.Verify(n => n.SendOrderConfirmationEmailAsync("alice@vip.com", It.IsAny<string>(), 864.00m), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_DecrementsInventoryStockCorrectly()
        {
            // Arrange: 10 laptops in stock, buy 3
            var model = new CreateOrderViewModel
            {
                CustomerId = 2,
                Items = new List<CreateOrderItemInput>
                {
                    new CreateOrderItemInput { ItemId = 1, Quantity = 3 }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeTrue();
            var updatedItem = await _context.Items.FindAsync(1);
            updatedItem!.StockQuantity.Should().Be(7); // 10 - 3 = 7
        }

        [Fact]
        public async Task CreateOrderAsync_WithInsufficientInventory_ReturnsFailureError()
        {
            // Arrange: 5 mice in stock, request 10
            var model = new CreateOrderViewModel
            {
                CustomerId = 2,
                Items = new List<CreateOrderItemInput>
                {
                    new CreateOrderItemInput { ItemId = 2, Quantity = 10 }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeFalse();
            result.Order.Should().BeNull();
            result.Message.Should().Contain("Insufficient inventory");
        }

        [Fact]
        public async Task CreateOrderAsync_WithNonExistentCustomerId_ReturnsFailureError()
        {
            // Arrange
            var model = new CreateOrderViewModel
            {
                CustomerId = 9999, // Unknown customer
                Items = new List<CreateOrderItemInput>
                {
                    new CreateOrderItemInput { ItemId = 1, Quantity = 1 }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Selected customer does not exist");
        }

        [Fact]
        public async Task CreateOrderAsync_WithInactiveOrInvalidItem_ReturnsFailureError()
        {
            // Arrange: Item 3 is inactive
            var model = new CreateOrderViewModel
            {
                CustomerId = 2,
                Items = new List<CreateOrderItemInput>
                {
                    new CreateOrderItemInput { ItemId = 3, Quantity = 1 }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("invalid or inactive");
        }

        [Fact]
        public async Task CreateOrderAsync_WithNullOrEmptyItemsList_ReturnsFailureError()
        {
            // Arrange
            var model = new CreateOrderViewModel
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemInput>()
            };

            // Act
            var result = await _orderService.CreateOrderAsync(model);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("at least one item");
        }

        [Fact]
        public async Task GetOrderDetailsAsync_ForExistingOrder_ReturnsCompleteSummary()
        {
            // Arrange: Create an order first
            var model = new CreateOrderViewModel
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemInput> { new CreateOrderItemInput { ItemId = 2, Quantity = 2 } }
            };
            var createResult = await _orderService.CreateOrderAsync(model);

            // Act
            var details = await _orderService.GetOrderDetailsAsync(createResult.Order!.OrderId);

            // Assert
            details.Should().NotBeNull();
            details!.CustomerName.Should().Be("Alice VIP");
            details.Items.Should().HaveCount(1);
            details.Items.First().ItemName.Should().Be("Wireless Mouse");
        }
    }
}
