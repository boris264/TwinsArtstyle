using TwinsArtstyle.Infrastructure.Models;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface ICartService
    {
        public Cart CreateCart();

        public Task DeleteCart(Guid cartId);
    }
}
