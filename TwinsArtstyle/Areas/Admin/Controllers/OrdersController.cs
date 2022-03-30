using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class OrdersController : AdminController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> All()
        {
            var orders = (await _orderService.GetOrdersForAdminArea()).OrderByDescending(o => o.CreationDate);
            return View(orders);
        }

        public async Task<IActionResult> Details([FromQuery] string orderId)
        {
            var order = await _orderService.GetAdminOrderById(orderId);

            if(order != null)
            {
                return View(order);
            }

            return RedirectToAction(nameof(All));
        }
    }
}
