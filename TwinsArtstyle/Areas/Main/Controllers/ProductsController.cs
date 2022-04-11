using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using TwinsArtstyle.Helpers;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    public class ProductsController : MainController
    {
        private readonly IProductService _productService;
        private readonly IDistributedCache _cache;

        public ProductsController(IProductService productService,
            IDistributedCache cache)
        {
            _productService = productService;
            _cache = cache;
        }

        public async Task<IActionResult> ByCategory([FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return NotFound();
            }

            var products = JsonHelper
                .Deserialize<IEnumerable<ProductViewModel>>(Encoding.Unicode.GetString(await _cache.GetAsync($"products")));

            if(products.Any(c => c.Category == category))
            {
                ViewData["Category"] = category;
                return View(products.Where(p => p.Category == category));
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(string productId)
        {
            var products = JsonHelper.
                Deserialize<IEnumerable<ProductViewModel>>(Encoding.Unicode.GetString(await _cache.GetAsync("products")));

            if (!products.Any(p => p.Id == productId))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(products.Where(p => p.Id == productId).FirstOrDefault());
        }
    }
}
