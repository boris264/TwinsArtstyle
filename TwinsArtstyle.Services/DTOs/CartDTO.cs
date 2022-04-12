using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.DTOs
{
    public class CartDTO
    {
        public IList<CartProductViewModel> Products { get; set; } = new List<CartProductViewModel>();
    }
}
