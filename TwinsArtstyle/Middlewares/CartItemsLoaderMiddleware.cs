using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Middlewares
{
    public class CartItemsLoaderMiddleware : IMiddleware
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartItemsLoaderMiddleware> _logger;
        private readonly IDistributedCache _cache;
        private readonly ICacheSerializer _cacheSerializer;

        public CartItemsLoaderMiddleware(ICartService cartService,
            ILogger<CartItemsLoaderMiddleware> logger,
            IDistributedCache cache,
            ICacheSerializer cacheSerializer)
        {
            _cartService = cartService;
            _logger = logger;
            _cache = cache;
            _cacheSerializer = cacheSerializer;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(context.User.Identity.IsAuthenticated)
            {
                var userCartId = context.User.FindFirst(ClaimType.CartId).Value;
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                try
                {
                    var cartForUserFromCache = await _cache.GetAsync(userCartId);

                    if (cartForUserFromCache == null)
                    {
                        var userCartItems = await _cartService.GetProductsForUser(userId);
                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(CacheConstants.CacheCartExpirationMinutes));
                        await _cache.SetAsync(userCartId, _cacheSerializer.SerializeToByteArray(userCartItems), options);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Couldn't load user's cart items into cache! Exception message: {e.Message}");
                }
            }

            await next(context);
        }
    }
}
