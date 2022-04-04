using Microsoft.AspNetCore.Mvc;
using System.IO;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class ProductsController : AdminController
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService,
            IWebHostEnvironment webHostEnv,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _webHostEnv = webHostEnv;
            _logger = logger;
        }

        public async Task<IActionResult> Manage()
        {
            var products = await _productService.GetProducts();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {

                if (productViewModel.Image?.Length > 0)
                {
                    var imageName = Path.GetFileName(productViewModel.Image.FileName);
                    var imagePath = $"{_webHostEnv.WebRootPath}/images/{imageName}";

                    using (var stream = System.IO.File.Create(imagePath))
                    {
                        await productViewModel.Image.CopyToAsync(stream);
                    }

                    productViewModel.ImageUrl = $"/images/{imageName}";

                    var result = await _productService.AddProduct(productViewModel);

                    if(result.Success)
                    {
                        return RedirectToAction(nameof(Manage));
                    }

                    _logger.LogError(result.ErrorMessage);
                    ModelState.AddModelError(String.Empty, "Something went wrong...");
                }
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
