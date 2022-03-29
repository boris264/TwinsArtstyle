using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels.OrderModels
{
    public class OrderDTO
    {
        [Required]
        public string AddressName { get; set; }
    }
}
