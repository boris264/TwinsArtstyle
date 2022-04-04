using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductViewModel>> GetProducts();

        public Task<OperationResult> AddProduct(ProductViewModel productViewModel);

        public Task<IEnumerable<ProductViewModel>> GetByCategory(string categoryName);

        public Task<bool> Exists(string productId);

        public Task<ProductViewModel> GetById(string productId);
    }
}
