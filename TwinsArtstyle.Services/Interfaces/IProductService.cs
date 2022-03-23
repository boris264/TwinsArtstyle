using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductViewModel>> GetProducts();

        public Task AddProduct(ProductViewModel productViewModel);

        public Task<IEnumerable<ProductViewModel>> GetByCategory(string categoryName);
    }
}
