using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
