using TwinsArtstyle.Services.Enums;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.ViewModels.OrderModels
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        public ICollection<CartProductViewModel> Products { get; set; }

        public string AddressName { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}
