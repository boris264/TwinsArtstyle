using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class AddressesController : MainController
    {
        private readonly IAddressService _addressService;
        private readonly UserManager<User> _userManager;

        public AddressesController(IAddressService addressService,
            UserManager<User> userManager)
        {
            _addressService = addressService;
            _userManager = userManager;
        }

        public async Task<IActionResult> All()
        {
            var addresses = await _addressService.GetAddressesForUser(_userManager.GetUserId(HttpContext.User));
            return View(addresses);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddressViewModel addressViewModel)
        {
            var result = await _addressService.AddNewAddress(addressViewModel, _userManager.GetUserId(User));

            if (result)
            {
                return RedirectToAction(nameof(All));
            }

            return View(addressViewModel);
        }
    }
}
