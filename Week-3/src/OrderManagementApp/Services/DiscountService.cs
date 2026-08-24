using OrderManagementApp.Models;

namespace OrderManagementApp.Services
{
    public interface IDiscountService
    {
        decimal CalculateTierDiscount(CustomerTier tier, decimal subtotal);
        (bool IsValid, decimal DiscountAmount, string Message) EvaluateCoupon(Coupon? coupon, decimal subtotal);
    }

    public class DiscountService : IDiscountService
    {
        public decimal CalculateTierDiscount(CustomerTier tier, decimal subtotal)
        {
            if (subtotal <= 0) return 0;

            return tier switch
            {
                CustomerTier.VIP => Math.Round(subtotal * 0.10m, 2),        // 10% VIP Discount
                CustomerTier.Enterprise => Math.Round(subtotal * 0.20m, 2), // 20% Enterprise Discount
                _ => 0m
            };
        }

        public (bool IsValid, decimal DiscountAmount, string Message) EvaluateCoupon(Coupon? coupon, decimal subtotal)
        {
            if (coupon == null)
            {
                return (false, 0, "Coupon does not exist.");
            }

            if (!coupon.IsActive)
            {
                return (false, 0, "This coupon is no longer active.");
            }

            if (coupon.ExpiryDate < DateTime.UtcNow)
            {
                return (false, 0, "This coupon has expired.");
            }

            if (subtotal < coupon.MinOrderAmount)
            {
                return (false, 0, $"Minimum order amount of ${coupon.MinOrderAmount:F2} required to apply this coupon.");
            }

            decimal calculatedDiscount = Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);

            // Apply max cap if configured
            if (coupon.MaxDiscountAmount > 0 && calculatedDiscount > coupon.MaxDiscountAmount)
            {
                calculatedDiscount = coupon.MaxDiscountAmount;
            }

            return (true, calculatedDiscount, "Coupon applied successfully!");
        }
    }
}
