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

        public Task<IEnumerable<CartProductViewModel>> GetProductsForUser(ClaimsPrincipal userClaims);
    }
}
