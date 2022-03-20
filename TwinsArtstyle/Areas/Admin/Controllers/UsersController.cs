using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class UsersController : AdminController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public UsersController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserService userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Registered()
        {
            IEnumerable<UserViewModel> users = await _userService.GetAllUsers();
            return View(users);
        }
    }
}
