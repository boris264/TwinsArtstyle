using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NUnit.Framework;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;
using TwinsArtstyle.Tests.Helpers;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class InboxServiceTests
    {
        private ApplicationDbContext dbContext;
        private IRepository repository;
        private IInboxService inboxService;
        private UserManager<User> userManager;

        [SetUp]
        public void Setup()
        {
            dbContext = InMemoryDbConfig.CreateDbContext("Inbox_Test_Database");
            repository = new Repository(dbContext);
            userManager = new UserManager<User>(new UserStore<User>(dbContext), null, null, null, null, null, null, null, null);
            inboxService = new InboxService(userManager, repository);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        [Test]
        public async Task CanAddContactLetter()
        {
            var userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            ContactUsViewModel contactUsViewModel = new ContactUsViewModel()
            {
                Title = "Hello",
                Content = "Test"
            };

            var result = await inboxService.AddContactLetter(contactUsViewModel, userId);
            Assert.That(result.Success, Is.True);
            Assert.That(dbContext.Messages.Any(m => m.Title == "Hello"));
        }

        [Test]
        public async Task CheckAddingContactLetterWithInvalidUserId()
        {
            var userId = Guid.NewGuid().ToString();
            ContactUsViewModel contactUsViewModel = new ContactUsViewModel()
            {
                Title = "Hello",
                Content = "Test"
            };

            var result = await inboxService.AddContactLetter(contactUsViewModel, userId);
            Assert.That(result.Success, Is.False);
            Assert.That(!dbContext.Messages.Any(m => m.Title == "Hello"));
            Assert.That(result.ErrorMessage, Is.EqualTo("User should not be null!"));
        }

        [Test]
        public async Task CanGetAllMessages()
        {
            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            var contactUsViewModel = new ContactUsViewModel()
            {
                Title = "Hello",
                Content = "Test"
            };
            var contactUsViewModel2 = new ContactUsViewModel()
            {
                Title = "Hello 2",
                Content = "Test t"
            };

            await inboxService.AddContactLetter(contactUsViewModel, userId);
            await inboxService.AddContactLetter(contactUsViewModel2, userId);

            var result = await inboxService.GetAllMessages();
            Assert.That(result.Count() == 2);
            Assert.That(result.Any(m => m.Title == "Hello" && m.Content == "Test"));
        }
    }
}
