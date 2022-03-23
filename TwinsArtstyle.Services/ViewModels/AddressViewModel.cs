using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class AddressViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string AddressText { get; set; }
    }
}
