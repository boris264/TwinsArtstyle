using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class ContactUsViewModel
    {

        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Content { get; set; } 
    }
}
