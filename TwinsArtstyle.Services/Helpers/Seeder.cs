using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;

namespace TwinsArtstyle.Services.Helpers
{
    public class Seeder
    {
        public async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                UserManager<User> userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                ILogger<Seeder> logger = scope.ServiceProvider.GetRequiredService<ILogger<Seeder>>();

                if (repository == null)
                {
                    throw new ArgumentNullException(nameof(repository));
                }

                await SeedAdminUser(userManager, logger);
                await SeedProducts(repository, logger);
            }
        }

        private async Task SeedAdminUser(UserManager<User> userManager, ILogger logger)
        {
            var firstName = "Admin";
            var lastName = "Administrator";
            var email = "admin@gmail.com";
            var password = "123456789";
            var phoneNumber = "0892222222";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                
                var user = new User()
                {
                    Cart = new Cart(),
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                };

                var result = await userManager.CreateAsync(user, password);

                if(result.Succeeded)
                {
                    logger.LogInformation("Successfully seeded an admin user!");
                }
                else
                {
                    logger.LogInformation($"Seeding of admin user failed! Errors: {string.Join(", ", result.Errors)}");
                }
            }

            logger.LogInformation("Admin user already exists.");
        }

        private async Task SeedProducts(IRepository repository, ILogger logger)
        {
            var productsFromDatabase = await repository.All<Product>()
                .ToListAsync();

            if(productsFromDatabase.Count != 32)
            {
                var productsFromJson = JsonHelper
                    .Deserialize<List<Product>>(await File.ReadAllTextAsync("Data/products.json"));
                await repository.AddRange(productsFromJson);
                await repository.SaveChanges();
            }
        }
    }
}
