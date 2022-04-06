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
    public class AddressServiceTests : ServiceTests
    {
        [Test]
        public async Task CheckAddressesForUser()
        {
            var addressService = new AddressService(repository);
            var addressesForUser = await addressService.GetAddressesForUser("03fcf816-15f5-4df5-adc8-16f6ff504f3d");

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
            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
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
            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
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

            string userId = "03fcf816-15f5-4df5-adc8-16f6ff504f3d";
            var addressService = new AddressService(repository);
            var result = await addressService.AddNewAddress(addressViewModel, userId);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.DbUpdateFailedMessage));
        }

        [Test]
        public async Task CheckIfAddressExistsForUser()
        {
            var addressService = new AddressService(repository);
            var resultUser1 = await addressService.AddressExistsForUser("Test 1", "03fcf816-15f5-4df5-adc8-16f6ff504f3d");
            var resultUser2 = await addressService.AddressExistsForUser("Test 3", "edd0019f-288a-4365-80e4-fbfd5aac7e72");
            Assert.That(resultUser1, Is.Not.Null);
            Assert.That(resultUser2, Is.Not.Null);
        }
    }
}
