using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    public class ProductsController : MainController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ByCategory([FromQuery] string category)
        {
            if(category == null)
            {
                return NotFound();
            }

            var productsByCategory = await _productService.GetByCategory(category);

            ViewData["Category"] = category;
            return View(productsByCategory);
        }

        public async Task<IActionResult> Details(string productId)
        {
            if(!await _productService.Exists(productId))
            {
                return Redirect("/");
            }

            var product = await _productService.GetById(productId);
            return View(product);
        }
    }
}
