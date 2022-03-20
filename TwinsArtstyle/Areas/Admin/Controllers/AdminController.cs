using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwinsArtstyle.Services.Constants;

namespace TwinsArtstyle.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleType.Admininstrator)]
    public abstract class AdminController : Controller
    {
    }
}
