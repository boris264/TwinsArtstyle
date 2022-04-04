using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
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
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService,
            UserManager<User> userManager,
            IAddressService addressService,
            IOrderService orderService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _addressService = addressService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var userCartIdClaim = HttpContext.User.FindFirst(ClaimType.CartId);
                var result = await _cartService.AddToCart(userCartIdClaim.Value, product.productId, product.count);

                if (result.Success)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var result = await _cartService
                .RemoveFromCart(product.productId, HttpContext.User.FindFirst(ClaimType.CartId).Value);

                if (result.Success)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userAddresses = await _addressService.GetAddressesForUser(user.Id);
            var products = await _cartService.GetProductsForUser(user.Id);

            if (user != null)
            {
                var orderViewModel = new PlaceOrderViewModel()
                {
                    Addresses = userAddresses,
                    Products = products,
                    TotalPrice = products.Sum(p => p.Count * p.Product.Price)
                };

                return View(orderViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDTO orderViewModel)
        {
            var result = await _orderService.
                Add(orderViewModel, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (result.Success)
            {
                await _cartService.CleanCart(HttpContext.User.FindFirst(ClaimType.CartId).Value);
                return RedirectToAction("Index", "Home");
            }

            _logger.LogError(result.ErrorMessage);
            return RedirectToAction(nameof(PlaceOrder));
        }
    }
}
