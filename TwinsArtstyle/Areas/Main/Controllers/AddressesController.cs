using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class AddressesController : MainController
    {
        public async Task<IActionResult> All()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Add()
        //{

        //}
    }
}
