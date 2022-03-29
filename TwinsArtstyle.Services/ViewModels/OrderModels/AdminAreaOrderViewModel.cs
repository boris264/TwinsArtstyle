using TwinsArtstyle.Services.Enums;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.ViewModels.OrderModels
{
    public class AdminAreaOrderViewModel
    {
        public Guid OrderId { get; set; }

        public UserViewModel User { get; set; }
        
        public string AddressText { get; set; }

        public decimal TotalPrice { get; set; }

        public ICollection<CartProductViewModel> Products { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
