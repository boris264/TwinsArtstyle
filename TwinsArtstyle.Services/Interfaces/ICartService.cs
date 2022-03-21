using TwinsArtstyle.Infrastructure.Models;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface ICartService
    {
        public Cart CreateCart();

        public Task DeleteCart(Guid cartId);

        public Task<bool> AddToCart(string cartId, string productId, int count);
    }
}
