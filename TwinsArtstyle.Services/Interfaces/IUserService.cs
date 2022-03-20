using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> UpdateUserProfileInfo(UserViewModel userViewModel, ClaimsPrincipal claimsPrincipal);
        public Task<IEnumerable<UserViewModel>> GetAllUsers();
    }
}
