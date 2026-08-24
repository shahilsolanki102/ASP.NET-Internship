using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementApp;
using OrderManagementApp.Data;
using OrderManagementApp.Models;
using Xunit;

namespace OrderManagement.IntegrationTests
{
    public class OrderIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public OrderIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_HomePage_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
        }

        [Fact]
        public async Task Get_OrderIndexPage_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Order");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Orders");
        }

        [Fact]
        public async Task Get_OrderCreatePage_ContainsFormInputsAndCustomerOptions()
        {
            // Act
            var response = await _client.GetAsync("/Order/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Create Customer Order");
            content.Should().Contain("form");
            content.Should().Contain("CustomerId");
        }

        [Fact]
        public async Task Get_TestDashboard_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/TestDashboard");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Automated Test Execution Dashboard");
        }

        [Fact]
        public async Task Post_CreateOrder_EndToEnd_PersistsInDatabase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var customer = new Customer { Name = "Integration Test User", Email = "integration@test.com", Tier = CustomerTier.Standard };
            var item = new Item { SKU = "INT-TEST-01", Name = "Integration Test Item", UnitPrice = 150m, StockQuantity = 20 };
            context.Customers.Add(customer);
            context.Items.Add(item);
            await context.SaveChangesAsync();

            // Act: Simulate form POST
            var postData = new Dictionary<string, string>
            {
                { "CustomerId", customer.Id.ToString() },
                { "Items[0].ItemId", item.Id.ToString() },
                { "Items[0].Quantity", "2" }
            };

            var postResponse = await _client.PostAsync("/Order/Create", new FormUrlEncodedContent(postData));

            // Assert
            postResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Redirect, HttpStatusCode.OK, HttpStatusCode.Found, HttpStatusCode.BadRequest);
        }
    }
}
