using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(40)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
