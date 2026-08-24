using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderManagementApp.Controllers;
using OrderManagementApp.Data;
using OrderManagementApp.Models;
using OrderManagementApp.Services;
using OrderManagementApp.ViewModels;
using Xunit;

namespace OrderManagement.UnitTests.Controllers
{
    public class OrderControllerTests : IDisposable
    {
        private readonly OrderDbContext _context;
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _orderServiceMock = new Mock<IOrderService>();
            
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider);

            _controller = new OrderController(_orderServiceMock.Object, _context)
            {
                TempData = tempData
            };

            SeedData();
        }

        private void SeedData()
        {
            _context.Customers.Add(new Customer { Id = 1, Name = "Test Customer", Email = "test@customer.com" });
            _context.Items.Add(new Item { Id = 1, SKU = "SKU-1", Name = "Test Item", UnitPrice = 100m, StockQuantity = 10 });
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfOrders()
        {
            // Arrange
            var sampleOrders = new List<OrderSummaryViewModel>
            {
                new OrderSummaryViewModel { OrderId = 1, OrderNumber = "ORD-001", TotalAmount = 150m }
            };
            _orderServiceMock.Setup(s => s.GetAllOrdersAsync(default)).ReturnsAsync(sampleOrders);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<List<OrderSummaryViewModel>>().Subject;
            model.Should().HaveCount(1);
        }

        [Fact]
        public async Task Create_GetAction_ReturnsViewWithPopulatedDropdowns()
        {
            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<CreateOrderViewModel>();
            var customers = (List<Customer>)_controller.ViewBag.Customers;
            customers.Should().NotBeNull();
            customers.Should().HaveCount(1);
        }

        [Fact]
        public async Task Create_PostAction_WithInvalidModelState_ReturnsViewWithoutCallingService()
        {
            // Arrange
            var model = new CreateOrderViewModel { CustomerId = 0 }; // Invalid
            _controller.ModelState.AddModelError("CustomerId", "Customer is required");

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(model);
            _orderServiceMock.Verify(s => s.CreateOrderAsync(It.IsAny<CreateOrderViewModel>(), default), Times.Never);
        }

        [Fact]
        public async Task Create_PostAction_WhenServiceSucceeds_RedirectsToDetails()
        {
            // Arrange
            var model = new CreateOrderViewModel
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemInput> { new CreateOrderItemInput { ItemId = 1, Quantity = 2 } }
            };
            var orderSummary = new OrderSummaryViewModel { OrderId = 101, OrderNumber = "ORD-101" };

            _orderServiceMock
                .Setup(s => s.CreateOrderAsync(model, default))
                .ReturnsAsync((true, orderSummary, "Order created successfully!"));

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(OrderController.Details));
            redirectResult.RouteValues!["id"].Should().Be(101);
        }

        [Fact]
        public async Task Details_WhenOrderNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            _orderServiceMock.Setup(s => s.GetOrderDetailsAsync(999, default)).ReturnsAsync((OrderSummaryViewModel?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
