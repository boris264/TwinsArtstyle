using TwinsArtstyle.Middlewares;

namespace TwinsArtstyle.Extensions
{
    public static class CartItemsLoaderMiddlewareExtension
    {
        public static IApplicationBuilder UseCartItemsLoader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CartItemsLoaderMiddleware>();
        }
    }
}
