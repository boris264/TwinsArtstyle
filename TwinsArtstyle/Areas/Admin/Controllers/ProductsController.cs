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

        public ProductsController(IProductService productService,
            IWebHostEnvironment webHostEnv)
        {
            _productService = productService;
            _webHostEnv = webHostEnv;
        }

        public async Task<IActionResult> Manage()
        {
            var products = await _productService.GetProducts();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(ProductViewModel productViewModel)
        {
            if(ModelState.IsValid)
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

                    try
                    {
                        await _productService.AddProduct(productViewModel);
                        return RedirectToAction(nameof(Manage));
                    }
                    catch (ArgumentNullException)
                    {
                        ModelState.AddModelError(String.Empty, "Something went wrong...");
                    }
                }
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
