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

            return View(productsByCategory);
        }
    }
}
