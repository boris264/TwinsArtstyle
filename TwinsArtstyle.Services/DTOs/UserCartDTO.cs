using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.DTOs
{
    public class UserCartDTO
    {
        public string UserId { get; set; }

        public Guid CartId { get; set; }

        public IList<CartProductViewModel> Products { get; set; }
    }
}
