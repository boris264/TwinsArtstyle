using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, string userId);

        public Task<IdentityResult> UpdateUserProfileByEmail(UserViewModel userViewModel, string email);
        public Task<IEnumerable<UserViewModel>> GetAllUsers();
        public Task<OperationResult> DeleteUser(string email);
        public Task<IEnumerable<UserViewModel>> GetUsersByRole(string roleName);
    }
}
