using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApp.Data;
using OrderManagementApp.Services;
using OrderManagementApp.ViewModels;

namespace OrderManagementApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly OrderDbContext _context;

        public OrderController(IOrderService orderService, OrderDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Items = await _context.Items.Where(i => i.IsActive).OrderBy(i => i.Name).ToListAsync();
            ViewBag.Coupons = await _context.Coupons.Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow).ToListAsync();

            var model = new CreateOrderViewModel
            {
                Items = new List<CreateOrderItemInput> { new CreateOrderItemInput() }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
                ViewBag.Items = await _context.Items.Where(i => i.IsActive).OrderBy(i => i.Name).ToListAsync();
                ViewBag.Coupons = await _context.Coupons.Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow).ToListAsync();
                return View(model);
            }

            var (success, order, message) = await _orderService.CreateOrderAsync(model);

            if (!success || order == null)
            {
                ModelState.AddModelError(string.Empty, message);
                ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
                ViewBag.Items = await _context.Items.Where(i => i.IsActive).OrderBy(i => i.Name).ToListAsync();
                ViewBag.Coupons = await _context.Coupons.Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow).ToListAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Details), new { id = order.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
