using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class OrdersController : AdminController
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
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

        public async Task<IActionResult> ChangeOrderStatus(string orderId, string newStatus)
        {
            var result = await _orderService.ChangeOrderStatus(orderId, newStatus);

            if(!result.Success)
                _logger.LogWarning(result.ErrorMessage);

            return RedirectToAction(nameof(Details), new { orderId = orderId });
        }
    }
}
