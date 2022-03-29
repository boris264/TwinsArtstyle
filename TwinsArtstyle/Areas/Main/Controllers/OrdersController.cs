using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [Authorize]
    public class OrdersController : MainController
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<User> _userManager;
        
        public OrdersController(UserManager<User> userManager,
            IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        public async Task<IActionResult> All()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var ordersForUser = await _orderService.GetOrdersForUser(userId);
            return View(ordersForUser);
        }

        public async Task<IActionResult> Details([FromQuery] string orderId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if(order != null)
            {
                return View(order);
            }

            return RedirectToAction(nameof(All));
        }
    }
}
