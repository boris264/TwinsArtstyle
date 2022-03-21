using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRepository repository;

        public CartService(IRepository repo)
        {
            repository = repo;
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
                    await repository.Add(new CartProductCount()
                    {
                        Cart = cart,
                        Product = product,
                        Count = count
                    });

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
    }
}
