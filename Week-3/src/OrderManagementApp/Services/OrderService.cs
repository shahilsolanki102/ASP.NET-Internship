using Microsoft.EntityFrameworkCore;
using OrderManagementApp.Data;
using OrderManagementApp.Models;
using OrderManagementApp.ViewModels;

namespace OrderManagementApp.Services
{
    public interface IOrderService
    {
        Task<(bool Success, OrderSummaryViewModel? Order, string Message)> CreateOrderAsync(CreateOrderViewModel model, CancellationToken cancellationToken = default);
        Task<OrderSummaryViewModel?> GetOrderDetailsAsync(int orderId, CancellationToken cancellationToken = default);
        Task<List<OrderSummaryViewModel>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    }

    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IDiscountService _discountService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderService> _logger;

        public const decimal TaxRate = 0.08m; // 8% standard sales tax

        public OrderService(
            OrderDbContext context,
            IDiscountService discountService,
            INotificationService notificationService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _discountService = discountService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<(bool Success, OrderSummaryViewModel? Order, string Message)> CreateOrderAsync(
            CreateOrderViewModel model, 
            CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return (false, null, "Order submission model cannot be null.");
            }

            if (model.Items == null || !model.Items.Any())
            {
                return (false, null, "Order must contain at least one item.");
            }

            // 1. Validate Customer
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.CustomerId, cancellationToken);
            if (customer == null)
            {
                return (false, null, "Selected customer does not exist.");
            }

            // 2. Fetch and Validate Items & Stock
            var itemIds = model.Items.Select(i => i.ItemId).Distinct().ToList();
            var dbItems = await _context.Items.Where(i => itemIds.Contains(i.Id) && i.IsActive).ToListAsync(cancellationToken);

            if (dbItems.Count != itemIds.Count)
            {
                return (false, null, "One or more requested items are invalid or inactive.");
            }

            decimal subtotal = 0m;
            var orderItemsList = new List<CustomerOrderItem>();

            foreach (var itemInput in model.Items)
            {
                if (itemInput.Quantity <= 0)
                {
                    return (false, null, "Item quantity must be greater than zero.");
                }

                var dbItem = dbItems.First(i => i.Id == itemInput.ItemId);

                // Stock validation
                if (dbItem.StockQuantity < itemInput.Quantity)
                {
                    return (false, null, $"Insufficient inventory for '{dbItem.Name}'. Requested: {itemInput.Quantity}, Available: {dbItem.StockQuantity}.");
                }

                decimal itemTotal = Math.Round(dbItem.UnitPrice * itemInput.Quantity, 2);
                subtotal += itemTotal;

                // Decrement inventory stock
                dbItem.StockQuantity -= itemInput.Quantity;

                orderItemsList.Add(new CustomerOrderItem
                {
                    ItemId = dbItem.Id,
                    UnitPrice = dbItem.UnitPrice,
                    Quantity = itemInput.Quantity,
                    TotalPrice = itemTotal
                });
            }

            // 3. Calculate Discounts
            decimal tierDiscount = _discountService.CalculateTierDiscount(customer.Tier, subtotal);
            decimal couponDiscount = 0m;
            string? appliedCouponCode = null;

            if (!string.IsNullOrWhiteSpace(model.CouponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == model.CouponCode.Trim().ToUpper(), cancellationToken);
                var (isValidCoupon, discountAmount, couponMessage) = _discountService.EvaluateCoupon(coupon, subtotal);
                if (!isValidCoupon)
                {
                    return (false, null, couponMessage);
                }
                couponDiscount = discountAmount;
                appliedCouponCode = coupon?.Code;
            }

            decimal totalDiscount = tierDiscount + couponDiscount;
            if (totalDiscount > subtotal)
            {
                totalDiscount = subtotal;
            }

            // 4. Calculate Taxes & Final Total
            decimal taxableAmount = subtotal - totalDiscount;
            decimal taxAmount = Math.Round(taxableAmount * TaxRate, 2);
            decimal totalAmount = taxableAmount + taxAmount;

            // 5. Build and Save Order Entity
            string orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            var order = new CustomerOrder
            {
                OrderNumber = orderNumber,
                CustomerId = customer.Id,
                Subtotal = subtotal,
                DiscountAmount = totalDiscount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                CouponCodeApplied = appliedCouponCode,
                Status = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItemsList
            };

            _context.CustomerOrders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            // 6. Send Notification
            await _notificationService.SendOrderConfirmationEmailAsync(customer.Email, orderNumber, totalAmount);

            var summary = new OrderSummaryViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                CustomerTier = customer.Tier,
                Subtotal = subtotal,
                DiscountAmount = totalDiscount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                CouponCodeApplied = appliedCouponCode,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = orderItemsList.Select(oi =>
                {
                    var item = dbItems.First(i => i.Id == oi.ItemId);
                    return new OrderItemDetailViewModel
                    {
                        ItemId = item.Id,
                        ItemName = item.Name,
                        SKU = item.SKU,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.TotalPrice
                    };
                }).ToList()
            };

            return (true, summary, "Order created successfully!");
        }

        public async Task<OrderSummaryViewModel?> GetOrderDetailsAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null) return null;

            return new OrderSummaryViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = order.Customer?.Name ?? "N/A",
                CustomerEmail = order.Customer?.Email ?? "N/A",
                CustomerTier = order.Customer?.Tier ?? CustomerTier.Standard,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                CouponCodeApplied = order.CouponCodeApplied,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDetailViewModel
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.Item?.Name ?? "N/A",
                    SKU = oi.Item?.SKU ?? "N/A",
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }

        public async Task<List<OrderSummaryViewModel>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking()
                .Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.Customer != null ? o.Customer.Name : "N/A",
                    CustomerEmail = o.Customer != null ? o.Customer.Email : "N/A",
                    CustomerTier = o.Customer != null ? o.Customer.Tier : CustomerTier.Standard,
                    Subtotal = o.Subtotal,
                    DiscountAmount = o.DiscountAmount,
                    TaxAmount = o.TaxAmount,
                    TotalAmount = o.TotalAmount,
                    CouponCodeApplied = o.CouponCodeApplied,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(oi => new OrderItemDetailViewModel
                    {
                        ItemId = oi.ItemId,
                        ItemName = oi.Item != null ? oi.Item.Name : "N/A",
                        SKU = oi.Item != null ? oi.Item.SKU : "N/A",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.TotalPrice
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
