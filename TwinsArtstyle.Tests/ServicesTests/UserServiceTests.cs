using NUnit.Framework;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class UserServiceTests : ServiceTests
    {
        private ICartService cartService;
        private IUserService userService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            cartService = new CartService(repository, userManager);
            userService = new UserService(userManager, repository, cartService);
        }

        [Test]
        public async Task TestUserDeletionByEmail()
        {
            var normalizedEmail = "GEORGISTOILOV@ABV.BG";
            var result = await userService.DeleteUser(normalizedEmail);
            TestContext.WriteLine(result.ErrorMessage);
            Assert.That(result.Success, Is.True);
            Assert.That(!dbContext.Users.Any(u => u.Email == normalizedEmail));
        }

        [Test]
        public async Task TestUserDeletionByInvalidEmail()
        {
            var normalizedEmail = "invalid";
            var result = await userService.DeleteUser(normalizedEmail);
 
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid email address!"));
        }

        [Test]
        public async Task TestRetrievalOfAllUsers()
        {
            var users = await userService.GetAllUsers();

            Assert.That(users.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task TestUpdatingAnUsersProfileInformation()
        {
            UserViewModel userViewModel = new UserViewModel()
            {
                Email = "goshoivanov@abv.bg",
                FirstName = "Gosho",
                LastName = "Ivanod",
                PhoneNumber = "0892222222"
            };

            var userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            var result = await userService.UpdateUserProfileInfo(userViewModel, userId);

            Assert.That(result.Succeeded , Is.True);
        }

        [Test]
        public async Task TestUpdatingAnUsersProfileInfoByEmail()
        {
            UserViewModel userViewModel = new UserViewModel()
            {
                Email = "goshoivanov@abv.bg",
                FirstName = "Gosho",
                LastName = "Ivanod",
                PhoneNumber = "0892222222"
            };

            string email = "BORISTODOROV12@GMAIL.COM";

            var result = await userService.UpdateUserProfileByEmail(userViewModel, email);

            Assert.That(result.Succeeded, Is.True);
        }
    }
}
