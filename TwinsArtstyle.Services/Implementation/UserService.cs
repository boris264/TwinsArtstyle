using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICartService _cartService;
        private readonly IRepository _repository;

        public UserService(UserManager<User> userManager,
            IRepository repository,
            ICartService cartService)
        {
            _userManager = userManager;
            _repository = repository;
            _cartService = cartService;
        }

        public async Task<bool> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            bool result = false;

            if (user != null)
            {
                await _repository.Remove(user);
                await _cartService.DeleteCart(user.CartId);
                result = true;
            }

            return result;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            IEnumerable<UserViewModel> users = await _repository.All<User>()
                .Select(u => new UserViewModel()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                }).ToListAsync();

            return users;
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersByRole(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.Select(u => new UserViewModel()
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber
            });
        }

        public async Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);

            if (user != null)
            {
                user.FirstName = userViewModel.FirstName;
                user.LastName = userViewModel.LastName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;

                return await _userManager.UpdateAsync(user);
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, string email)
        {
            IdentityResult result = IdentityResult.Failed();

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    user.FirstName = userViewModel.FirstName;
                    user.LastName = userViewModel.LastName;
                    user.Email = userViewModel.Email;
                    user.PhoneNumber = userViewModel.PhoneNumber;

                    return await _userManager.UpdateAsync(user);
                }
            }

            return result;
        }
    }
}
