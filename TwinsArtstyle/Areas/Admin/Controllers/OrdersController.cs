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
            var orders = await _orderService.GetOrdersForAdminArea();
            return View(orders.OrderByDescending(o => o.CreationDate));
        }
    }
}
