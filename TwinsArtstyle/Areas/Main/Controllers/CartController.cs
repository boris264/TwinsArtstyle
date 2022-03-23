using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    public class CartController : MainController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDTO product)
        {
            var userCartIdClaim = HttpContext.User.FindFirst(ClaimType.CartId);
            var result = await _cartService.AddToCart(userCartIdClaim.Value, product.productId, product.count);
            
            if(result)
            {
                return Ok();
            }

            return BadRequest();
        }

    }
}
