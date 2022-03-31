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
    }
}
