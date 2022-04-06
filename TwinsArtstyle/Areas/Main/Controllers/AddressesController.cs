using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class AddressesController : MainController
    {
        private readonly IAddressService _addressService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IAddressService addressService,
            UserManager<User> userManager,
            ILogger<AddressesController> logger)
        {
            _addressService = addressService;
            _userManager = userManager;
            _logger = logger;
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
            if (ModelState.IsValid)
            {
                OperationResult result =
                await _addressService.AddNewAddress(addressViewModel, 
                HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (result.Success)
                {
                    return RedirectToAction(nameof(All));
                }

                _logger.LogError(result.ErrorMessage);
            }

            ModelState.AddModelError(string.Empty, Messages.UnexpectedErrorOccured);
            return View(addressViewModel);
        }
    }
}
