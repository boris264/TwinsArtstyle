using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.ViewModels.OrderModels
{
    public class PlaceOrderViewModel
    {
        public IEnumerable<AddressViewModel> Addresses { get; set; }

        public IEnumerable<CartProductViewModel> Products { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
