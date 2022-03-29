using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class AdministratorsController : AdminController
    {
        private readonly UserManager<User> _userManager;

        public AdministratorsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null && !await _userManager.IsInRoleAsync(user, RoleType.Admininstrator))
            {
                await _userManager.AddToRoleAsync(user, RoleType.Admininstrator);
                return RedirectToAction(nameof(Add));
            }

            return View();
        }
    }
}
