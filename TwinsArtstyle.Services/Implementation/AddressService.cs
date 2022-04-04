using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Helpers;
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

        public async Task<OperationResult> AddNewAddress(AddressViewModel addressViewModel, string userId)
        {
            OperationResult result = new OperationResult();
            var user = await _repository.All<User>()
                .Include(u => u.Addresses)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            try
            {
                if (!user.Addresses.Any(a => a.Name == addressViewModel.Name
                     && a.AddressText == addressViewModel.AddressText))
                {
                    user.Addresses.Add(new Address()
                    {
                        Name = addressViewModel.Name,
                        AddressText = addressViewModel.AddressText
                    });

                    await _repository.SaveChanges();
                    result.Success = true;
                }

            }
            catch (ArgumentNullException)
            {
                result.ErrorMessage = "Address name and/or text shouldn't be empty!";
            }
            catch(DbUpdateException)
            {
                result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
            }

            return result;
        }

        public async Task<AddressViewModel> AddressExistsForUser(string addressName, string userId)
        {
            return await _repository.All<Address>()
                .Where(a => a.UserId == userId && a.Name == addressName)
                .Select(a => new AddressViewModel()
                {
                    Name = a.Name,
                    AddressText = a.AddressText
                })
                .FirstOrDefaultAsync();
        }
    }
}
