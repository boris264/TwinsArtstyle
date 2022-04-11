using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Helpers
{
    public static class CacheConfigurator
    {
        public static async Task ConfigureRedisCache(IServiceProvider serviceProvider)
        {
            if(serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            using (var scope = serviceProvider.CreateScope())
            {
                var productService = scope.ServiceProvider.GetService<IProductService>();
                var cache = scope.ServiceProvider.GetService<IDistributedCache>();

                if (productService == null)
                {
                    throw new InvalidOperationException($"Failed to initialize {nameof(productService)} from the container!");
                }

                if (cache == null)
                {
                    throw new InvalidOperationException($"Failed to initialize {nameof(cache)} from the container!");
                }

                if (await cache.GetAsync("products") == null)
                {
                    var allProducts = await productService.GetProducts();
                    var allProductsJson = JsonHelper.Serialize(allProducts);
                    var allProductsBytes = Encoding.Unicode.GetBytes(allProductsJson);
                    var cacheProductsOptions = new DistributedCacheEntryOptions();
                    await cache.SetAsync("products", allProductsBytes);
                }
            }
        }
    }
}
