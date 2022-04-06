using NUnit.Framework;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.OrderModels;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class OrderServiceTests : ServiceTests
    {
        private ICartService cartService;
        private IOrderService orderService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            cartService = new CartService(repository, userManager);
            orderService = new OrderService(repository, cartService, userManager);
        }

        [Test]
        public async Task CheckCreationOfOrder()
        {
            OrderDTO orderDTO = new OrderDTO()
            {
                AddressName = "Test 1"
            };

            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";

            var result = await orderService.Add(orderDTO, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(dbContext.Orders.Any(o => o.User.Id == userId));
        }

        [Test]
        public async Task CheckOrderRetrievalForUser()
        {
            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            var result = await orderService.GetOrdersForUser(userId);
            Assert.That(result.Count() == 1);
            Assert.That(result.FirstOrDefault()?.AddressName == "Test 1");
        }

        [Test]
        public async Task CheckCalculationOfOrderTotalPrice()
        {
            string orderId = "1446938e-d101-4505-b748-dc25f71b93bd";
            decimal totalPrice = await orderService.CalculateTotalPrice(orderId);
            Assert.That(totalPrice, Is.EqualTo(45.00M));
        }

        [Test]
        public async Task CheckCalculationOfOrderTotalPriceWithInvalidOrderId()
        {
            string orderId = "wdaddwawdawda";
            decimal totalPrice = await orderService.CalculateTotalPrice(orderId);
            Assert.That(totalPrice, Is.EqualTo(0));
        }

        [Test]
        public async Task CheckRetrievingAnOrderById()
        {
            string orderId = "1446938e-d101-4505-b748-dc25f71b93bd";
            var result = await orderService.GetOrderById(orderId);
            Assert.That(result.Id.ToString() == orderId);
        }

        [Test]
        public async Task CheckRetrievingAnOrderByInvalidId()
        {
            string orderId = "dawwdadwa";
            var result = await orderService.GetOrderById(orderId);
            Assert.That(result, Is.Null);
        }
    }
}
