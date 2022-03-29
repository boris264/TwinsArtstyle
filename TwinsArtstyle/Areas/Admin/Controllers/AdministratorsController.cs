using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class AdministratorsController : AdminController
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public AdministratorsController(UserManager<User> userManager,
            IUserService userService)
        {
            _userManager = userManager;
            _userService = userService; 
        }

        public async Task<IActionResult> Add()
        {
            var administrators = await _userService.GetUsersByRole(RoleType.Admininstrator);
            
            return View(administrators);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] string email)
        {
            string errorMessage = "Invalid email!";

            if(!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null && !await _userManager.IsInRoleAsync(user, RoleType.Admininstrator))
                {
                    await _userManager.AddToRoleAsync(user, RoleType.Admininstrator);
                    return RedirectToAction(nameof(Add));
                }

                errorMessage = "User with this email was not found, or the user is already an Administrator!";
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction(nameof(Add));
        }
    }
}
