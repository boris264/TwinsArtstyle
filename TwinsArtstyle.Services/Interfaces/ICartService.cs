using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface ICartService
    {
        public Task<Cart> CreateCart();

        public Task<OperationResult> DeleteCart(Guid cartId);

        public Task<OperationResult> AddToCart(string cartId, string productId, int count);

        public Task<IEnumerable<CartProductViewModel>> GetProductsForUser(string userId);
        public Task<IEnumerable<CartProductViewModel>> GetProductsForUser(ClaimsPrincipal user);

        public Task<OperationResult> RemoveFromCart(string productId, string cartId);

        public Task<decimal> CalculateTotalPrice(string cartId);

        public Task<OperationResult> CleanCart(string cartId);
    }
}
