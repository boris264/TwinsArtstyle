using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRepository repository;
        private readonly UserManager<User> _userManager;

        public CartService(IRepository repo,
            UserManager<User> userManager)
        {
            repository = repo;
            _userManager = userManager;
        }

        public Cart CreateCart()
        {
            return new Cart();
        }

        public async Task DeleteCart(Guid cartId)
        {
            var cart = repository.All<Cart>()
                .Where(c => c.Id == cartId)
                .FirstOrDefault();

            if(cart != null)
            {
                await repository.Remove(cart);
            }
        }

        public async Task<bool> AddToCart(string cartId, string productId, int count)
        {
            bool result = false;
            var cart = repository.All<Cart>()
                .Where(c => c.Id.ToString() == cartId)
                .FirstOrDefault();
            var product = repository.All<Product>()
                .Where(p => p.Id.ToString() == productId)
                .FirstOrDefault();

            if(cart != null && product != null && count > 0)
            {
                try
                {
                    var cartProduct = repository.All<CartProductCount>()
                        .Where(c => c.Cart == cart && c.Product == product)
                        .FirstOrDefault();

                    if(cartProduct != null)
                    {
                        cartProduct.Count += count;
                    }
                    else
                    {
                        await repository.Add(new CartProductCount()
                        {
                            Cart = cart,
                            Product = product,
                            Count = count
                        });
                    }
                   
                    await repository.SaveChanges();
                    result = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    result = false;
                }
                catch(DbUpdateException)
                {
                    result = false;
                }
            }

            return result;
        }

        public async Task<IEnumerable<CartProductViewModel>> GetProductsForUser(ClaimsPrincipal userClaims)
        {
            var userCartIdClaim = userClaims.FindFirst(ClaimType.CartId);

            if(userCartIdClaim != null)
            {
                var userCartId = userCartIdClaim.Value;

                var items = await repository.All<CartProductCount>()
                    .Where(c => c.CartId.ToString() == userCartId)
                    .Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Name = p.Product.Name,
                            ImageUrl = p.Product.ImageUrl,
                            Price = p.Product.Price,
                            Category = p.Product.Category.Name
                        },
                        Count = p.Count
                    }).ToListAsync();

                return items;
            }

            return Enumerable.Empty<CartProductViewModel>();
        }
    }
}
