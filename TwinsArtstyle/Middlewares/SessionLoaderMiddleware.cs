using System.Security.Claims;
using System.Text;
using TwinsArtstyle.Helpers;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.DTOs;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Middlewares
{
    public class SessionLoaderMiddleware : IMiddleware
    {
        private readonly ILogger<SessionLoaderMiddleware> _logger;
        private readonly ICartService _cartService;

        public SessionLoaderMiddleware(ILogger<SessionLoaderMiddleware> logger,
            ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                if (!context.Session.IsAvailable)
                {
                    await context.Session.LoadAsync();
                }

                if (context.Session.Get(userId) == null)
                {
                    var userProductItems = await _cartService.GetProductsForUser(context.User);

                    var userCartDTO = new CartDTO()
                    {
                        CartId = new Guid(context.User.FindFirst(ClaimType.CartId).Value),
                        Products = new List<CartProductViewModel>(userProductItems)
                    };

                    context.Session.Set(userId, Encoding.Unicode.GetBytes(JsonHelper.Serialize(userCartDTO)));
                }
            }

            await next(context);
        }
    }
}
