using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.DTOs;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;
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
                    await AddProductToCartCache(product, userCartId);
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
                    await RemoveProductFromCache(product, userCartId);
                    return Ok();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAddresses = await _addressService.GetAddressesForUser(user.Id);
            var userCartDTO = _cacheSerializer
                .DeserializeFromByteArray<CartDTO>(await _cache.GetAsync(user.CartId.ToString()));

            if (user != null)
            {
                var orderViewModel = new PlaceOrderViewModel()
                {
                    Addresses = userAddresses,
                    Products = userCartDTO.Products,
                    TotalPrice = userCartDTO.Products.Sum(p => p.Count * p.Product.Price)
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
                await _cache.SetAsync(userCartId, _cacheSerializer.SerializeToByteArray(new CartDTO()));

                return RedirectToAction("Index", "Home");
            }

            _logger.LogError(result.ErrorMessage);
            return RedirectToAction(nameof(PlaceOrder));
        }

        private async Task AddProductToCartCache(ProductDTO productDTO, string cartId)
        {
            var productFromCacheBytes = await _cache.GetAsync("products");

            if(productFromCacheBytes != null)
            {
                var productsFromCache = _cacheSerializer
                    .DeserializeFromByteArray<IEnumerable<ProductViewModel>>(productFromCacheBytes);

                var cartFromCache = _cacheSerializer.DeserializeFromByteArray<CartDTO>(await _cache.GetAsync(cartId));

                var productToAdd = productsFromCache.Where(p => p.Id == productDTO.productId)
                    .FirstOrDefault();

                var cartProductExists = cartFromCache.Products.Where(p => p.Product.Id == productToAdd.Id)
                        .FirstOrDefault();

                if (cartProductExists != null)
                {
                    cartProductExists.Count += productDTO.count;
                }
                else
                {
                    cartFromCache.Products.Add(new CartProductViewModel()
                    {
                        Product = productToAdd,
                        Count = productDTO.count
                    });
                }

                await _cache.SetAsync(cartId, _cacheSerializer.SerializeToByteArray(cartFromCache));
            }
        }

        private async Task RemoveProductFromCache(ProductDTO productDTO, string cartId)
        {
            var cartFromCache = _cacheSerializer.DeserializeFromByteArray<CartDTO>(await _cache.GetAsync(cartId));
            var productToRemove = cartFromCache.Products.Where(p => p.Product.Id == productDTO.productId)
                .FirstOrDefault();
            
            if(cartFromCache.Products.Remove(productToRemove))
            {
                await _cache.SetAsync(cartId, _cacheSerializer.SerializeToByteArray(cartFromCache));
            }
        }
    }
}
