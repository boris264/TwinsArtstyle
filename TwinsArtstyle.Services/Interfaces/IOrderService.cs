using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels.OrderModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<OperationResult> Add(OrderDTO orderViewModel, string userId);

        public Task<IEnumerable<OrderViewModel>> GetOrdersForUser(string userId);

        public Task<decimal> CalculateTotalPrice(string orderId);

        public Task<OrderViewModel> GetOrderById(string orderId);

        public Task<IEnumerable<AdminAreaOrderViewModel>> GetOrdersForAdminArea();

        public Task<AdminAreaOrderViewModel> GetAdminOrderById(string orderId);

        public Task<OperationResult> ChangeOrderStatus(string orderId, string newStatus);
    }
}
