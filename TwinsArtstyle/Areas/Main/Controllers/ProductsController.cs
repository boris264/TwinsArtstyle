﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    public class ProductsController : MainController
    {
        private readonly IDistributedCache _cache;
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly ICacheSerializer _cacheSerializer;

        public ProductsController(IDistributedCache cache,
            IProductService productService,
            ILogger<ProductsController> logger,
            ICacheSerializer cacheSerializer)
        {
            _cache = cache;
            _productService = productService;
            _logger = logger;
            _cacheSerializer = cacheSerializer;
        }

        public async Task<IActionResult> ByCategory([FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return NotFound();
            }

            var products = _cacheSerializer
                .DeserializeFromByteArray<IEnumerable<ProductViewModel>>(await _cache.GetAsync($"products"));

            if (products != null)
            {
                if (products.Any(c => c.Category == category))
                {
                    ViewData["Category"] = category;
                    return View(products.Where(p => p.Category == category));
                }
            }
            else
            {
                _logger.LogWarning("Fetching products from the cache returned 0 products! Check if the cache is working properly.");
                var productsFromDb = await _productService.GetByCategory(category);
                return View(productsFromDb);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(string productId)
        {
            var products = _cacheSerializer
                .DeserializeFromByteArray<IEnumerable<ProductViewModel>>(await _cache.GetAsync("products"));

            if (products != null)
            {
                if (!products.Any(p => p.Id == productId))
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(products.Where(p => p.Id == productId).FirstOrDefault());
            }
            else
            {
                _logger.LogWarning("Couldn't fetch product from cache! Fetching it from the database instead...");

                var productExists = await _productService.Exists(productId);

                if (productExists)
                {
                    var product = await _productService.GetById(productId);
                    return View(product);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
