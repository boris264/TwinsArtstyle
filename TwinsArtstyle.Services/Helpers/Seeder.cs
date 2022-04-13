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
using TwinsArtstyle.Services.Constants;

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
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (repository == null)
                {
                    throw new ArgumentNullException(nameof(repository));
                }

                await SeedRoles(roleManager);
                await SeedCategories(repository);
                await SeedAdminUser(userManager, logger);
                await SeedProducts(repository);
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

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleType.Admininstrator);
                    logger.LogInformation("Successfully seeded an admin user!");
                }
                else
                {
                    logger.LogInformation($"Seeding of admin user failed! Errors: {string.Join(", ", result.Errors)}");
                }
            }

            logger.LogInformation("Admin user already exists.");
        }

        private async Task SeedProducts(IRepository repository)
        {
            var productsFromDatabase = await repository.All<Product>()
                .ToListAsync();

            if (productsFromDatabase.Count != 32)
            {
                var productsFromJson = JsonHelper
                    .Deserialize<List<Product>>(await File.ReadAllTextAsync("Data/products.json"));
                await repository.AddRange(productsFromJson);
                await repository.SaveChanges();
            }
        }

        private async Task SeedCategories(IRepository repository)
        {
            var categories = await repository.All<Category>()
                .ToListAsync();

            // The id's are fixed, because i generated the products.json file from the already existing entities in the database
            // and i didn't want to edit the json file.

            if (categories.Count != 3)
            {
                var print = new Category()
                {
                    Id = new Guid("71e1b3a8-e3ec-4039-a6b1-28b7e92c8930"),
                    Name = "Принтове",
                };

                var drawing = new Category()
                {
                    Id = new Guid("05c84b4e-089c-4334-89bf-8bd29546eac7"),
                    Name = "Рисунки",
                };

                var canvas = new Category()
                { 
                    Id = new Guid("efcfe35b-c08a-4915-9321-1f6e6d4d258a"),
                    Name = "Принтове"
                };

                await repository.Add(print);
                await repository.Add(drawing);
                await repository.Add(canvas);
                await repository.SaveChanges();
            }
        }

        private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync(RoleType.User) ||
                !await roleManager.RoleExistsAsync(RoleType.User))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleType.User));
                await roleManager.CreateAsync(new IdentityRole(RoleType.Admininstrator));
            }
        }
    }
}
