using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Models;

namespace TwinsArtstyle.Tests.Helpers
{
    public static class InMemoryDbConfig
    {
        public static ApplicationDbContext CreateDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var dbContext = new ApplicationDbContext(options);
            
            dbContext.Categories.AddRange(GenerateCategoriesData());
            dbContext.Categories.
                Select(c => c.Products.
                Concat(dbContext.Products.Where(p => p.CategoryId == c.Id)));
            
            dbContext.Products.AddRange(GenerateProductsData());
            dbContext.Carts.AddRange(GenerateCartsData());
            dbContext.Addresses.AddRange(GenerateAddressesData());
            dbContext.Users.AddRange(GenerateUsersData());

            dbContext.Users
                .Select(u => u.Addresses
                .Concat(dbContext.Addresses.Where(a => a.UserId == u.Id)));
            dbContext.Orders.AddRange(GenerateOrdersData());
            dbContext.SaveChanges();
            return dbContext;
        }

        private static IEnumerable<Order> GenerateOrdersData()
        {
            return ParseJsonTestData<IEnumerable<Order>>("TestData/orders.json");
        }

        private static IEnumerable<Cart> GenerateCartsData()
        {
            return ParseJsonTestData<IEnumerable<Cart>>("TestData/carts.json");
        }

        private static IEnumerable<Category> GenerateCategoriesData()
        {
            return ParseJsonTestData<IEnumerable<Category>>("TestData/categories.json");
        }

        private static IEnumerable<User> GenerateUsersData()
        {
            return ParseJsonTestData<IEnumerable<User>>("TestData/users.json");
        }

        private static IEnumerable<Address> GenerateAddressesData()
        {
            return ParseJsonTestData<IEnumerable<Address>>("TestData/addresses.json");
        }

        private static IEnumerable<Product> GenerateProductsData()
        {
            return ParseJsonTestData<IEnumerable<Product>>("TestData/products.json");
        }

        private static T ParseJsonTestData<T>(string path)
        {
            var jsonReader = new StreamReader(File.OpenRead(path));
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonReader reader = new JsonTextReader(jsonReader);
            return jsonSerializer.Deserialize<T>(reader);
        }
    }
}
