using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class ProductsController : AdminController
    {
        private readonly int productImageWidth = 200;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly ILogger<ProductsController> _logger;
        private readonly IDistributedCache _cache;
        private readonly ICacheSerializer _cacheSerializer;

        public ProductsController(IProductService productService,
            IWebHostEnvironment webHostEnv,
            ILogger<ProductsController> logger,
            IDistributedCache cache,
            ICacheSerializer cacheSerializer)
        {
            _productService = productService;
            _webHostEnv = webHostEnv;
            _logger = logger;
            _cache = cache;
            _cacheSerializer = cacheSerializer;

        }

        public async Task<IActionResult> Manage()
        {
            // This is not good, but it does the job for now. Will fix it when i implement my own IDistributedCache.

            var products = await _productService.GetProducts();
            await _cache.SetAsync("products", _cacheSerializer.SerializeToByteArray(products));

            return View(products);
        }

        public async Task<IActionResult> Delete([FromQuery] string productId)
        {
            if (!string.IsNullOrEmpty(productId))
            {
                var products = _cacheSerializer
                    .DeserializeFromByteArray<IEnumerable<ProductViewModel>>(await _cache.GetAsync("products"))
                    .ToList();

                if (products == null)
                {
                    _logger.LogError("Couldn't get products from cache! Fetching from database...");
                    products = (await _productService.GetProducts()).ToList();
                }

                var productById = products.Where(p => p.Id == productId).FirstOrDefault();

                if (productById != null)
                {
                    var result = await _productService.DeleteById(productId);

                    if (result.Success)
                    {
                        products.Remove(productById);

                        await _cache.SetAsync("products", _cacheSerializer.SerializeToByteArray(products));

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
                    var requestImageStream = productViewModel.Image.OpenReadStream();

                    // We try to reduce the image dimension, in order to reduce it's size and because
                    // some of the images i intend to upload here are with pretty large dimensions, making
                    // them larger in size, which slows down the loading of the web pages.

                    try
                    {
                        var newImageBytes = ReduceImageDimensions(requestImageStream, imagePath);
                        await System.IO.File.WriteAllBytesAsync(imagePath, newImageBytes);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning($"Couldn't save and reduce image's dimensions! Exception message: {e.Message}");
                        _logger.LogWarning("Saving original image instead...");

                        try
                        {
                            using (var stream = System.IO.File.Create(imagePath))
                            {
                                await productViewModel.Image.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Couldn't save product image! Aborting... | Exception message: {ex.Message}");
                            return RedirectToAction(nameof(Manage));
                        }
                    }

                    productViewModel.ImageUrl = $"/images/{imageName}";

                    var result = await _productService.AddProduct(productViewModel);

                    if (result.Success)
                    {
                        return RedirectToAction(nameof(Manage));
                    }

                    _logger.LogError(result.ErrorMessage);
                    ModelState.AddModelError(String.Empty, Messages.UnexpectedErrorOccured);
                }
            }

            return View(productViewModel);
        }

        private byte[] ReduceImageDimensions(Stream requestImageStream, string savePath)
        {
            using (Bitmap image = new Bitmap(requestImageStream))
            {
                using (MemoryStream imageStream = new MemoryStream())
                {
                    if (image.Width > productImageWidth)
                    {
                        int newResponsiveHeightForImage = (productImageWidth * image.Height) / image.Width;

                        using (Bitmap newImage = new Bitmap(productImageWidth, newResponsiveHeightForImage))
                        {
                            newImage.SetResolution(100, 100);

                            using (Graphics g = Graphics.FromImage(newImage))
                            {
                                g.Clear(Color.White);
                                g.DrawImage(image, new Rectangle(0, 0, productImageWidth, newResponsiveHeightForImage),
                                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                                newImage.Save(imageStream, image.RawFormat);
                            }
                        }
                    }
                    var imageBytes = imageStream.ToArray();

                    return imageBytes;
                }
            }
        }
    }
}
