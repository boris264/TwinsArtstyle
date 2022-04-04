using Moq;
using NUnit.Framework;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Implementation;
using TwinsArtstyle.Services.Interfaces;
using Microsoft.EntityFrameworkCore.InMemory;
using TwinsArtstyle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TwinsArtstyle.Services.ViewModels;
using TwinsArtstyle.Services.Constants;

namespace TwinsArtstyle.Tests.ServicesTests
{
    [TestFixture]
    public class AddressServiceTests
    {
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private IRepository repository;

        [OneTimeSetUp]
        public void SetupInMemoryDatabase()
        {
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Users_And_Addresses")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);
            var addresses = GetAddressesData();
            dbContext.Addresses.AddRange(addresses);
            dbContext.Users.AddRange(GetUsersData());
            dbContext.Users
                .Select(u => u.Addresses.Concat(addresses.Where(a => a.UserId == u.Id)));
            dbContext.SaveChanges();
            repository = new Repository(dbContext);
        }

        [Test]
        public async Task CheckAddressesForUser()
        {
            var addressService = new AddressService(repository);
            var addressesForUser = await addressService.GetAddressesForUser("c194bf04-b556-4fd3-9bca-674b9683514d");

            Assert.That(addressesForUser.Any(n => n.Name == "Test 1"));
        }

        [Test]
        public async Task CheckAddresses_ForUser_WhenSuppliedWithEmptyId()
        {
            var addressService = new AddressService(repository);
            var addressesForUser = await addressService.GetAddressesForUser(string.Empty);
            Assert.That(addressesForUser.Count() == 0);
        }

        [Test]
        public async Task CheckAddressesForUserWhenSuppliedWithNullId()
        {
            var addressService = new AddressService(repository);
            var addressesForUser = await addressService.GetAddressesForUser(null);
            Assert.IsNotNull(addressesForUser);
        }

        [Test]
        public async Task CheckAddAddressToUser()
        {
            AddressViewModel addressViewModel = new AddressViewModel()
            {
                Name = "test address",
                AddressText = "New York"
            };
            string userId = "c194bf04-b556-4fd3-9bca-674b9683514d";
            var addressService = new AddressService(repository);
            var result = await addressService.AddNewAddress(addressViewModel, userId);
            Assert.That(result.Success, Is.Not.False);
        }

        [Test]
        public async Task CheckAddingExistingAddressToUser()
        {
            AddressViewModel addressViewModel = new AddressViewModel()
            {
                Name = "Test 1",
                AddressText = "Test 1 text"
            };
            string userId = "c194bf04-b556-4fd3-9bca-674b9683514d";
            var addressService = new AddressService(repository);
            var result = await addressService.AddNewAddress(addressViewModel, userId);
            Assert.That(result.ErrorMessage, Is.EqualTo("Address already exists for that user!"));
        }

        [Test]
        public async Task CheckAddingAddressToWrongUserId()
        {
            AddressViewModel addressViewModel = new AddressViewModel()
            {
                Name = "Wrong user id",
                AddressText = "Wrong text"
            };

            string userId = "invalid guid";
            var addressService = new AddressService(repository);
            var result = await addressService.AddNewAddress(addressViewModel, userId);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid user Id was provided!"));
        }

        [Test]
        public async Task CheckAddingEmptyAddressToUser()
        {
            AddressViewModel addressViewModel = new AddressViewModel();

            string userId = "c194bf04-b556-4fd3-9bca-674b9683514d";
            var addressService = new AddressService(repository);
            var result = await addressService.AddNewAddress(addressViewModel, userId);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.DbUpdateFailedMessage));
        }

        [Test]
        public async Task CheckIfAddressExistsForUser()
        {
            var addressService = new AddressService(repository);
            var resultUser1 = await addressService.AddressExistsForUser("Test 1", "c194bf04-b556-4fd3-9bca-674b9683514d");
            var resultUser2 = await addressService.AddressExistsForUser("Test 3", "ecfe30b2-dfd4-4723-a8f8-77a7330ac8db");
            Assert.That(resultUser1, Is.Not.Null);
            Assert.That(resultUser2, Is.Not.Null);
        }

        public IEnumerable<Address> GetAddressesData()
        {
            return new List<Address>()
            {
                new Address()
                {
                    Name = "Test 1",
                    AddressText = "Test 1 text",
                    UserId = "c194bf04-b556-4fd3-9bca-674b9683514d"
                },
                new Address()
                {
                    Name = "Test 2",
                    AddressText = "Test 2 text",
                    UserId = "c194bf04-b556-4fd3-9bca-674b9683514d"
                },
                new Address()
                {
                    Name = "Test 3",
                    AddressText = "Test 3 text",
                    UserId = "ecfe30b2-dfd4-4723-a8f8-77a7330ac8db"
                },
                new Address()
                {
                    Name = "Test 4",
                    AddressText = "Test 4 text",
                    UserId = "ecfe30b2-dfd4-4723-a8f8-77a7330ac8db"
                },
            };
        }

        public IEnumerable<User> GetUsersData()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = "c194bf04-b556-4fd3-9bca-674b9683514d",
                    FirstName = "Boris",
                    LastName = "Todorov",
                    Email = "boristodorov1343@abv.bg",
                },
                new User()
                {
                    Id = "ecfe30b2-dfd4-4723-a8f8-77a7330ac8db",
                    FirstName = "Ivan",
                    LastName = "Georgiev",
                    Email = "ivangeorgiev64@gmail.com",
                }
            };
        }
    }
}
