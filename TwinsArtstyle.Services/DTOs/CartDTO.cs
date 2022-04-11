using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.DTOs
{
    public class CartDTO
    {
        public Guid CartId { get; set; }

        public IList<CartProductViewModel> Products { get; set; } = new List<CartProductViewModel>();
    }
}
