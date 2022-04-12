using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.OrderModels;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class CartController : MainController
    {
        private readonly ICartService _cartService;
        private readonly IAddressService _addressService;
        private readonly UserManager<User> _userManager;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;
        private readonly IDistributedCache _cache;
        private readonly ICacheSerializer _cacheSerializer;

        public CartController(ICartService cartService,
            UserManager<User> userManager,
            IAddressService addressService,
            IOrderService orderService,
            ILogger<CartController> logger,
            IDistributedCache cache,
            IProductService productService,
            ICacheSerializer cacheSerializer)
        {
            _cartService = cartService;
            _userManager = userManager;
            _addressService = addressService;
            _orderService = orderService;
            _logger = logger;
            _cache = cache;
            _productService = productService;
            _cacheSerializer = cacheSerializer;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var userCartId = HttpContext.User.FindFirst(ClaimType.CartId).Value;
                var result = await _cartService.AddToCart(userCartId, product.productId, product.count);

                if (result.Success)
                {
                    try
                    {
                        await AddProductToCartCache(product, userCartId);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Unexpected error occured while trying to save data into cache! Exception message: {e.Message}");
                    }

                    return Ok();
                }

                _logger.LogError(result.ErrorMessage);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var userCartId = HttpContext.User.FindFirst(ClaimType.CartId).Value;
                var result = await _cartService
                    .RemoveFromCart(product.productId, HttpContext.User.FindFirst(ClaimType.CartId).Value);

                if (result.Success)
                {
                    try
                    {
                        await RemoveProductFromCache(product, userCartId);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Unexpected error occured while trying to remove data from cache! Exception message: {e.Message}");
                    }

                    return Ok();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAddresses = await _addressService.GetAddressesForUser(user.Id);
            var userCartProducts = _cacheSerializer
                .DeserializeFromByteArray<IEnumerable<CartProductViewModel>>(await _cache.GetAsync(user.CartId.ToString()));

            // If the user items are not found in the cache, we try to load them from the database instead.

            if (userCartProducts == null)
            {
                _logger.LogWarning("Couldn't load user's cart items from cache! Loading them from database instead...");
                userCartProducts = await _cartService.GetProductsForUser(user.Id);
            }

            // If loading from the database returns 0 items, then the user's cart is empty, and we just redirect him.

            if (userCartProducts.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user != null)
            {
                var orderViewModel = new PlaceOrderViewModel()
                {
                    Addresses = userAddresses,
                    Products = userCartProducts,
                    TotalPrice = userCartProducts.Sum(p => p.Count * p.Product.Price)
                };

                return View(orderViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDTO orderViewModel)
        {
            var userCartId = HttpContext.User.FindFirst(ClaimType.CartId).Value;
            var result = await _orderService.
                Add(orderViewModel, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (result.Success)
            {
                await _cartService.CleanCart(userCartId);
                await _cache.SetAsync(userCartId, _cacheSerializer.SerializeToByteArray(new List<CartProductViewModel>()));

                return RedirectToAction("Index", "Home");
            }

            _logger.LogError(result.ErrorMessage);
            return RedirectToAction(nameof(PlaceOrder));
        }

        private async Task AddProductToCartCache(ProductDTO productDTO, string cartId)
        {
            var products = _cacheSerializer
                    .DeserializeFromByteArray<IEnumerable<ProductViewModel>>(await _cache.GetAsync("products"));

            if(products == null)
            {
                _logger.LogWarning("Couldn't load products from cache! Loading from database instead...");
                products = await _productService.GetProducts();
            }

            var cartFromCache = _cacheSerializer
                .DeserializeFromByteArray<IEnumerable<CartProductViewModel>>(await _cache.GetAsync(cartId))
                .ToList();

            var productToAdd = products.Where(p => p.Id == productDTO.productId)
                .FirstOrDefault();

            var cartProductExists = cartFromCache.Where(p => p.Product.Id == productToAdd.Id)
                    .FirstOrDefault();

            if (cartProductExists != null)
            {
                cartProductExists.Count += productDTO.count;
            }
            else
            {
                cartFromCache.Add(new CartProductViewModel()
                {
                    Product = productToAdd,
                    Count = productDTO.count
                });
            }

            await _cache.SetAsync(cartId, _cacheSerializer.SerializeToByteArray(cartFromCache));
        }

        private async Task RemoveProductFromCache(ProductDTO productDTO, string cartId)
        {
            var cartFromCache = _cacheSerializer
                .DeserializeFromByteArray<IEnumerable<CartProductViewModel>>(await _cache.GetAsync(cartId))
                .ToList();
            var productToRemove = cartFromCache.Where(p => p.Product.Id == productDTO.productId)
                .FirstOrDefault();

            if (cartFromCache.Remove(productToRemove))
            {
                await _cache.SetAsync(cartId, _cacheSerializer.SerializeToByteArray(cartFromCache));
            }
        }
    }
}
