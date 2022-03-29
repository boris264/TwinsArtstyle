using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface ICartService
    {
        public Cart CreateCart();

        public Task DeleteCart(Guid cartId);

        public Task<bool> AddToCart(string cartId, string productId, int count);

        public Task<IEnumerable<CartProductViewModel>> GetProductsForUser(string userId);
        public Task<IEnumerable<CartProductViewModel>> GetProductsForUser(ClaimsPrincipal user);

        public Task<bool> RemoveFromCart(string productId, string cartId);

        public Task<decimal> CalculateTotalPrice(string cartId);

        public Task<bool> CleanCart(string cartId);
    }
}
