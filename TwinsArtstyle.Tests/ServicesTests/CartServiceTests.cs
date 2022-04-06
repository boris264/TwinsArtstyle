using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class CartServiceTests : ServiceTests
    {
        private ICartService cartService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            cartService = new CartService(repository, userManager);
        }

        [Test]
        public async Task CheckCreationOfCart()
        {
            var cart = await cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);
            Assert.That(dbContext.Carts.Contains(cart));
        }

        [Test]
        public async Task CheckDeletionOfCart()
        {
            var cartId = new Guid("1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5");
            var result = await cartService.DeleteCart(cartId);
            Assert.That(result.Success, Is.True);
            Assert.That(!dbContext.Carts.Any(c => c.Id == cartId));
        }

        [Test]
        public async Task CheckDeletionOfCartWithNonExistingGuid()
        {
            var result = await cartService.DeleteCart(new Guid());
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid Cart Id!"));
        }

        [Test]
        public async Task CheckAddingAProductToACart()
        {
            var productId = "3203980b-1021-46f9-966b-70f13c4dc5d6";
            var cartId = "1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5";
            var result = await cartService.AddToCart(cartId, productId, 3);
            Assert.That(result.Success, Is.True);
            Assert.That(dbContext.CartsProductsCount
                .Any(cp => cp.ProductId.ToString() == productId && cp.CartId.ToString() == cartId && cp.Count == 3));
        }

        [Test]
        public async Task CheckAddingInvalidProductToCart()
        {
            var cartId = "1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5";
            var productId = new Guid();
            var result = await cartService.AddToCart(cartId, productId.ToString(), 3);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid Cart and/or Product Id!"));
        }

        [Test]
        public async Task CheckAddingProductToInvalidCartId()
        {
            var cartId = new Guid();
            var productId = "3203980b-1021-46f9-966b-70f13c4dc5d6";
            var result = await cartService.AddToCart(cartId.ToString(), productId.ToString(), 3);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid Cart and/or Product Id!"));
        }

        [Test]
        public async Task CheckProductsForUser()
        {
            var userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            var cartId = "42f486e2-8e8d-4cdb-afd7-b15ecfd7b5c5";
            var productId = "3203980b-1021-46f9-966b-70f13c4dc5d6";
            await cartService.AddToCart(cartId, productId, 1);
            var result = await cartService.GetProductsForUser(userId);
            Assert.That(result.Count() == 1);
        }

        [Test]
        public async Task CalculatePriceForCart()
        {
            var firstProductId = "1fc61825-03e6-4446-9375-5b8bf75e2d83";
            var secondProductId = "3203980b-1021-46f9-966b-70f13c4dc5d6";
            var cartId = "42f486e2-8e8d-4cdb-afd7-b15ecfd7b5c5";
            await cartService.AddToCart(cartId, firstProductId, 1);
            await cartService.AddToCart(cartId, secondProductId, 1);
            var totalPrice = await cartService.CalculateTotalPrice(cartId);
            Assert.That(totalPrice, Is.EqualTo(49));
        }

        [Test]
        public async Task CalculatePriceForEmptyOrInvalidCart()
        {
            var cartId = "42f486e2-8e8d-4cdb-afd7-b15ecfd7b5c5";
            var totalPrice = await cartService.CalculateTotalPrice(cartId);
            var totalPriceForInvalidCart = await cartService.CalculateTotalPrice("");
            Assert.That(totalPrice, Is.EqualTo(0));
            Assert.That(totalPriceForInvalidCart, Is.EqualTo(0));
        }

        [Test]
        public async Task CheckRemovingProductsFromCart()
        {
            var productId = "3203980b-1021-46f9-966b-70f13c4dc5d6";
            var cartId = "1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5";
            await cartService.AddToCart(cartId, productId, 1);
            var result = await cartService.RemoveFromCart(productId, cartId);
            Assert.That(result.Success, Is.True);
            Assert.That(!dbContext.CartsProductsCount
                .Any(a => a.ProductId.ToString() == productId && a.CartId.ToString() == cartId));
        }

        [Test]
        public async Task CheckRemovingInvalidProductFromCart()
        {
            var productId = Guid.NewGuid().ToString();
            var cartId = "1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5";
            var result = await cartService.RemoveFromCart(productId, cartId);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid Cart and/or Product Id!"));
        }

        [Test]
        public async Task CheckCleanCart()
        {
            var cartId = "1f93f0f9-5eeb-490a-a65c-1eafd8e36fa5";
            var result = await cartService.CleanCart(cartId);
            Assert.That(result.Success, Is.True);
            Assert.That(!dbContext.CartsProductsCount.Any(c => c.CartId.ToString() == cartId));
        }

        [Test]
        public async Task CheckCleanInvalidCart()
        {
            var cartId = Guid.NewGuid().ToString();
            var result = await cartService.CleanCart(cartId);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid Cart Id!"));
        }
    }
}
