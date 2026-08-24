using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationApp.Services;

namespace PerformanceOptimizationApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, string? search = null, int? categoryId = null)
        {
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentCategory"] = categoryId;
            ViewBag.Categories = await _productService.GetCachedCategoriesAsync();

            var paginatedProducts = await _productService.GetProductsOptimizedAsync(pageIndex, 15, search, categoryId);
            return View(paginatedProducts);
        }

        [HttpGet]
        public async Task<IActionResult> TopSelling()
        {
            var report = await _productService.GetTopSellingReportOptimizedAsync(15);
            return View(report);
        }
    }
}
