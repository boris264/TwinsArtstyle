using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class InboxController : AdminController
    {
        private readonly IInboxService _inboxService;

        public InboxController(IInboxService inboxService)
        {
            _inboxService = inboxService;
        }

        public async Task<IActionResult> All()
        {
            var messages = await _inboxService.GetAllMessages();
            return View(messages);
        }

        public async Task<IActionResult> Details(string messageId)
        {
            var message = await _inboxService.GetMessage(messageId);

            if(message != null)
            {
                return View(message);
            }

            return RedirectToAction(nameof(All));
        }
    }
}
