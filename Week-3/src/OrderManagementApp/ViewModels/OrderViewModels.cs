using System.ComponentModel.DataAnnotations;
using OrderManagementApp.Models;

namespace OrderManagementApp.ViewModels
{
    public class CreateOrderItemInput
    {
        [Required(ErrorMessage = "Item is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid item")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;
    }

    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Customer is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid customer")]
        public int CustomerId { get; set; }

        [MaxLength(30)]
        public string? CouponCode { get; set; }

        [MinLength(1, ErrorMessage = "An order must contain at least one item")]
        public List<CreateOrderItemInput> Items { get; set; } = new List<CreateOrderItemInput>();
    }

    public class OrderItemDetailViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public CustomerTier CustomerTier { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CouponCodeApplied { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDetailViewModel> Items { get; set; } = new List<OrderItemDetailViewModel>();
    }
}
