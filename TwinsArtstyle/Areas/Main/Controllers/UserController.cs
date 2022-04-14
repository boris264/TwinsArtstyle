using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Areas.Main.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class UserController : MainController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginViewModel> _logger;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ICartService _cartService;
        private readonly IEmailSender _emailSender;
        private readonly IDistributedCache _cache;
        private readonly ICacheSerializer _cacheSerializer;

        public UserController(SignInManager<User> signInManager,
            ILogger<LoginViewModel> logger,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            ICartService cartService,
            IEmailSender emailSender,
            IDistributedCache cache,
            ICacheSerializer cacheSerializer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _cartService = cartService;
            _emailSender = emailSender;
            _logger = logger;
            _cache = cache;
            _cacheSerializer = cacheSerializer;
        }

        public string ReturnUrl { get; set; }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            ReturnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.
                    PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    await LoadUserItemsIntoCache(loginModel.Email);

                    _logger.LogInformation("User logged in.");

                    return LocalRedirect(ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, Messages.InvalidLoginAttempt);
            }

            return View(loginModel);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Cart = await _cartService.CreateCart();

                await _userStore.SetUserNameAsync(user, registerModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, registerModel.Email, CancellationToken.None);
                await _userManager.SetPhoneNumberAsync(user, registerModel.PhoneNumber);
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddClaimAsync(user, new Claim(ClaimType.FullName, $"{user.FirstName} {user.LastName}"));
                    await _userManager.AddClaimAsync(user, new Claim(ClaimType.CartId, user.CartId.ToString()));
                    await _userManager.AddToRoleAsync(user, RoleType.User);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = registerModel.Email, returnUrl = "/Home/Index" });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, Messages.RegistrationFailed);
            }

            return View(registerModel);
        }

        public async Task<IActionResult> Logout()
        {
            var userCartId = User.FindFirst(ClaimType.CartId).Value;
            await _signInManager.SignOutAsync();
            await _cache.RemoveAsync(userCartId);

            return RedirectToAction("Index", "Home");
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }

        private async Task LoadUserItemsIntoCache(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var userCartProducts = await _cartService.GetProductsForUser(user.Id);
            var options = new DistributedCacheEntryOptions();

            options.SetSlidingExpiration(TimeSpan.FromMinutes(CacheConstants.CacheCartExpirationMinutes));
            await _cache
                    .SetAsync(user.CartId.ToString(), _cacheSerializer.SerializeToByteArray(userCartProducts), options);
        }
    }
}
