﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Text;
using TwinsArtstyle.Helpers;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class ProductsController : AdminController
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly ILogger<ProductsController> _logger;
        private readonly IDistributedCache _cache;

        public ProductsController(IProductService productService,
            IWebHostEnvironment webHostEnv,
            ILogger<ProductsController> logger,
            IDistributedCache cache)
        {
            _productService = productService;
            _webHostEnv = webHostEnv;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> Manage()
        {
            var products = await _productService.GetProducts();
            await _cache.SetAsync("products", Encoding.Unicode.GetBytes(JsonHelper.Serialize(products)));
            
            return View(products);
        }

        public async Task<IActionResult> Delete([FromQuery] string productId)
        {
            if(!string.IsNullOrEmpty(productId))
            {
                var products = JsonHelper
                    .Deserialize<IEnumerable<ProductViewModel>>
                    (Encoding.Unicode.GetString(await _cache.GetAsync("products"))).ToList();
                var productById = products.Where(p => p.Id == productId).FirstOrDefault();

                if(productById != null)
                {
                    var result = await _productService.DeleteById(productId);
                    
                    if(result.Success)
                    {
                        products.Remove(productById);

                        await _cache.SetAsync("products", Encoding.Unicode.GetBytes(JsonHelper.Serialize(products)));

                        return RedirectToAction(nameof(Manage));
                    }

                    _logger.LogError(result.ErrorMessage);
                }
            }

            return RedirectToAction(nameof(Manage));
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
                    ModelState.AddModelError(String.Empty, Messages.UnexpectedErrorOccured);
                }
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
