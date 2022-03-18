using Microsoft.AspNetCore.Mvc;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class AccountController : AdminController
    {
        public IActionResult Login() => View();
    }
}
