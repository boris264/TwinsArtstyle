using System.Text;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Middlewares;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<CartItemsLoaderMiddleware>();
            services.AddSingleton<ICacheSerializer, CacheSerializer>(x => new CacheSerializer(Encoding.Unicode));
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IInboxService, InboxService>();
            return services;
        }
    }
}
