using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class AccountController : AdminController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
    }
}
