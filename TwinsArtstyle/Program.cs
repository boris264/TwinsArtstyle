using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TwinsArtstyle.Extensions;
using TwinsArtstyle.Helpers;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(options =>
{
    options.AddConfiguration(builder.Configuration);
    options.AddConsole();

}).CreateLogger("Console");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "TwinsArtstyle.Session";
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(CookieConstants.SessionCookieExpirationMinutes);
});


builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Main/Home/AccessDenied";
    options.LoginPath = "/Main/User/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(CookieConstants.IdentityCookieExpirationMinutes);
});

builder.Services.AddApplicationServices();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await CacheConfigurator.ConfigureRedisCache(app.Services);
        logger.LogInformation("Successfully loaded cached data into redis.");
    }
    catch (Exception e)
    {
        logger.LogCritical($"Failed to load data into the Redis cache store! Exception message: {e.Message}\n");
        await app.StopAsync();
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseSessionLoader();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute("Main", "Main", "{area=Main}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute("Admin", "Admin", "Admin/{controller=Home}/{action=Info}/{id?}");

app.Run();
