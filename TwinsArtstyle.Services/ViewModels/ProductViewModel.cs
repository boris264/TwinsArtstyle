using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public IFormFile? Image { get; set; }

        [Url]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(30)]
        public string Category { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [Range(0, 200)]
        public decimal Price { get; set; }
    }
}
