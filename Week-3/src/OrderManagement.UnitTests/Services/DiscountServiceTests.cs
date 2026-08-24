using FluentAssertions;
using OrderManagementApp.Models;
using OrderManagementApp.Services;
using Xunit;

namespace OrderManagement.UnitTests.Services
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _discountService;

        public DiscountServiceTests()
        {
            _discountService = new DiscountService();
        }

        [Fact]
        public void CalculateTierDiscount_ForVIPCustomer_Returns10PercentDiscount()
        {
            // Arrange
            decimal subtotal = 500.00m;

            // Act
            decimal discount = _discountService.CalculateTierDiscount(CustomerTier.VIP, subtotal);

            // Assert
            discount.Should().Be(50.00m); // 10% of 500
        }

        [Fact]
        public void CalculateTierDiscount_ForEnterpriseCustomer_Returns20PercentDiscount()
        {
            // Arrange
            decimal subtotal = 1000.00m;

            // Act
            decimal discount = _discountService.CalculateTierDiscount(CustomerTier.Enterprise, subtotal);

            // Assert
            discount.Should().Be(200.00m); // 20% of 1000
        }

        [Fact]
        public void CalculateTierDiscount_ForStandardCustomer_ReturnsZero()
        {
            // Arrange
            decimal subtotal = 750.00m;

            // Act
            decimal discount = _discountService.CalculateTierDiscount(CustomerTier.Standard, subtotal);

            // Assert
            discount.Should().Be(0m);
        }

        [Fact]
        public void CalculateTierDiscount_ForZeroOrNegativeSubtotal_ReturnsZero()
        {
            // Act & Assert
            _discountService.CalculateTierDiscount(CustomerTier.VIP, 0m).Should().Be(0m);
            _discountService.CalculateTierDiscount(CustomerTier.VIP, -100m).Should().Be(0m);
        }

        [Fact]
        public void EvaluateCoupon_WithNullCoupon_ReturnsInvalid()
        {
            // Act
            var result = _discountService.EvaluateCoupon(null, 200m);

            // Assert
            result.IsValid.Should().BeFalse();
            result.DiscountAmount.Should().Be(0m);
            result.Message.Should().Contain("does not exist");
        }

        [Fact]
        public void EvaluateCoupon_WithInactiveOrExpiredCoupon_ReturnsInvalid()
        {
            // Arrange
            var inactiveCoupon = new Coupon { Code = "INACTIVE", IsActive = false, ExpiryDate = DateTime.UtcNow.AddDays(10), DiscountPercentage = 10 };
            var expiredCoupon = new Coupon { Code = "EXPIRED", IsActive = true, ExpiryDate = DateTime.UtcNow.AddDays(-5), DiscountPercentage = 10 };

            // Act
            var inactiveResult = _discountService.EvaluateCoupon(inactiveCoupon, 200m);
            var expiredResult = _discountService.EvaluateCoupon(expiredCoupon, 200m);

            // Assert
            inactiveResult.IsValid.Should().BeFalse();
            expiredResult.IsValid.Should().BeFalse();
            expiredResult.Message.Should().Contain("expired");
        }

        [Fact]
        public void EvaluateCoupon_WithValidCoupon_AppliesPercentageAndCapsAtMaximum()
        {
            // Arrange
            var couponWithCap = new Coupon 
            { 
                Code = "CAP50", 
                IsActive = true, 
                ExpiryDate = DateTime.UtcNow.AddDays(30), 
                DiscountPercentage = 50, 
                MinOrderAmount = 100,
                MaxDiscountAmount = 50.00m 
            };

            // Act
            var result = _discountService.EvaluateCoupon(couponWithCap, 500m); // 50% of 500 = 250, capped at 50

            // Assert
            result.IsValid.Should().BeTrue();
            result.DiscountAmount.Should().Be(50.00m);
        }
    }
}
