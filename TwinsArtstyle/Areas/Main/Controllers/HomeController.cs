using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    public class HomeController : MainController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IInboxService _inboxService;

        public HomeController(ILogger<HomeController> logger,
            IInboxService inboxService)
        {
            _logger = logger;
            _inboxService = inboxService;
        }

        public IActionResult Index()
        {
            Console.WriteLine("test");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult ContactUs()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsViewModel contactUsViewModel)
        {
            if(ModelState.IsValid)
            {
                await _inboxService
                    .AddContactLetter(contactUsViewModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
                return RedirectToAction(nameof(Index));
            }

            return View(contactUsViewModel);
        }
    }
}