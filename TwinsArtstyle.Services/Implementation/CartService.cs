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
    }
}
