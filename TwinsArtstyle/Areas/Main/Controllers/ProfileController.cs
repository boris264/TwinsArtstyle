using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class ProfileController : MainController
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public ProfileController(UserManager<User> userManager,
            IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            UserViewModel userModel = new UserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.
                    UpdateUserProfileInfo(model, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (result.Succeeded)
                {
                    ViewData["UpdateSuccessfull"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(UserProfile));
                }
            }

            ModelState.AddModelError(string.Empty, "Profile update failed!");
            return View(model);
        }
    }
}
