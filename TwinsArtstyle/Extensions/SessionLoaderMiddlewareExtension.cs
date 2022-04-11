using TwinsArtstyle.Middlewares;

namespace TwinsArtstyle.Extensions
{
    public static class SessionLoaderMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionLoader(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<SessionLoaderMiddleware>();
        }
    }
}
