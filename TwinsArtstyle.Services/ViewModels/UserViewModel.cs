﻿using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Services.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [StringLength(40)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(40)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^(\+359|0) *(87|88|89|98) *[0-9] *[0-9]{6}$", ErrorMessage = "Invalid phone number!")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
