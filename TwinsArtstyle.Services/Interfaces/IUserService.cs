using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, ClaimsPrincipal claimsPrincipal);

        public Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, string email);
        public Task<IEnumerable<UserViewModel>> GetAllUsers();
        public Task<bool> DeleteUser(string email);
        public Task<IEnumerable<UserViewModel>> GetUsersByRole(string roleName);
    }
}
