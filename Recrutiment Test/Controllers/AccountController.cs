using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Recrutiment_Test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Recrutiment_Test.Controllers
{
    public class AccountController : Controller
    {
        private readonly RecruitmentDbContext context;
        private readonly ILogger<HomeController> _logger;
        public static PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();
        public static List<string> Roles = new List<string>()
        {
            "Employee",
            "HR Manager",
            "Project Manager",
            "Administrator",
        };

        public AccountController(RecruitmentDbContext context, ILogger<HomeController> logger)
        {
            this.context = context;
            this._logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                
                var user = await context.AppUsers.FirstOrDefaultAsync(p => p.UserName == username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
                var result = hasher.VerifyHashedPassword(user, user.PasswordHashed, password);
                if(result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, Roles[user.Role]),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {username} logged in at {Time}.",
                        user.UserName, DateTime.UtcNow);

                    return RedirectToAction("Index", "Home");
                }
                
            }

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
    }
}
