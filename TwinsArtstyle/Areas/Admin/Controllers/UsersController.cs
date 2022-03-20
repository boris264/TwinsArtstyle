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

        public async Task<IActionResult> Edit([FromQuery] string email)
        {
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var userViewModel = new UserViewModel()
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber
                    };

                    return View(userViewModel);
                }
            }

            return RedirectToAction("Registered");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model, [FromQuery] string oldEmail)
        {
            var result = await _userService.UpdateUserProfileInfo(model, oldEmail);

            if(result.Succeeded)
            {
                return RedirectToAction("Edit", new {email = model.Email});
            }

            ModelState.AddModelError(string.Empty, "Something went wrong!");
            return View(model);
        }

        public async Task<IActionResult> Delete([FromQuery] string email)
        {
            await _userService.DeleteUser(email);
            return RedirectToAction("Registered");
        }
    }
}
