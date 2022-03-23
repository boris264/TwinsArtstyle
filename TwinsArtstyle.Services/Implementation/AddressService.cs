using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class AddressService : IAddressService
    {
        private readonly IRepository _repository;
        private readonly UserManager<User> _userManager;

        public AddressService(IRepository repository,
            UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<AddressViewModel>> GetAddressesForUser(string userId)
        {
            return await _repository.All<Address>()
                .Where(a => a.UserId == userId)
                .Select(a => new AddressViewModel()
                {
                    Name = a.Name,
                    AddressText = a.AddressText
                }).ToListAsync();
        }

        public async Task<bool> AddNewAddress(AddressViewModel addressViewModel, string userId)
        {
            var result = false;
            var user = await _userManager.FindByIdAsync(userId);

            try
            {
                user.Addresses.Add(new Address()
                {
                    Name = addressViewModel.Name,
                    AddressText = addressViewModel.AddressText
                });

                await _repository.SaveChanges();
                result = true;
            }
            catch (Exception)
            {

            }

            return result;
        }
    }
}
