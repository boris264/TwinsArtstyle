using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        public HomeController(UserManager<User> userManager)
        {

        }

        public IActionResult Info()
        {
            return View();
        }
    }
}
