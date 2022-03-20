using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    public class RolesController : AdminController
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Manage()
        {
            var allRoles = await GetAllRoles();

            return View(allRoles);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(RoleViewModel roleViewModel)
        { 
            if(ModelState.IsValid)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleViewModel.Name));
            }

            return RedirectToAction(nameof(Manage));
        }

        private async Task<List<RoleViewModel>> GetAllRoles()
        {
            return await _roleManager.Roles
                .Select(r => new RoleViewModel()
                {
                    Name = r.Name,
                }).ToListAsync();
        }
    }
}
