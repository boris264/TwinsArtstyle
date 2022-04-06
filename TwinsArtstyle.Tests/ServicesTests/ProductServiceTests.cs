using NUnit.Framework;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class ProductServiceTests : ServiceTests
    {
        private IProductService productService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            productService = new ProductService(repository);
        }

        [Test]
        public async Task CheckAddingANewProduct()
        {
            ProductViewModel product = new ProductViewModel()
            {
                ImageUrl = "https://google.com",
                Name = "Test product",
                Description = "Test product, for testing!!!",
                Price = 100.00M,
                Category = "Рисунки"
            };

            var result = await productService.AddProduct(product);
            Assert.That(result.Success , Is.True);
            Assert.That(dbContext.Products.Any(p => p.Name == "Test product"));
        }

        [Test]
        public async Task CheckAddingANewProductWithInvalidCategory()
        {
            ProductViewModel product = new ProductViewModel()
            {
                ImageUrl = "https://google.com",
                Name = "Test product",
                Description = "Test product, for testing!!!",
                Price = 100.00M,
                Category = "Invalid"
            };

            var result = await productService.AddProduct(product);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid category!"));
        }

        [Test]
        public async Task CheckAddingInvalidProduct()
        {
            ProductViewModel product = new ProductViewModel()
            {
                Category = "Рисунки"
            };

            var result = await productService.AddProduct(product);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.DbUpdateFailedMessage));
        }

        [Test]
        public async Task CheckRetrievalOfAllProducts()
        {
            var products = await productService.GetProducts();
            Assert.That(products.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task CheckRetrievalOfAllProductsByCategory()
        {
            var products = await productService.GetByCategory("Рисунки");
            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task CheckRetrievalOfAllProductsByInvalidCategory()
        {
            var products = await productService.GetByCategory("Invalid");
            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task CheckIfProductWithIdExists()
        {
            var id = "1fc61825-03e6-4446-9375-5b8bf75e2d83";
            var exists = await productService.Exists(id);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task CheckIfProductWithInvalidIdExistsReturnsFalse()
        {
            var id = Guid.NewGuid().ToString();
            var exists = await productService.Exists(id);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task CheckRetrievalOfProductById()
        {
            var id = "1fc61825-03e6-4446-9375-5b8bf75e2d83";
            var product = await productService.GetById(id);
            Assert.That(product.Name, Is.EqualTo("Risunka 1"));
        }

        [Test]
        public async Task CheckRetrievalOfProductByInvalidId()
        {
            var id = Guid.NewGuid().ToString();
            var product = await productService.GetById(id);
            Assert.That(product, Is.Null);
        }
    }
}
