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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Recrutiment_Test.Controllers
{
    public class AccountController : Controller
    {
        private readonly RecruitmentDbContext context;
        private readonly ILogger<HomeController> _logger;
        public static PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();
        public static List<string> Roles = new List<string>()
        {
            "HR Manager",
            "Project Manager",
            "Employee",
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
                if (user == null || !user.Active)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
                var result = hasher.VerifyHashedPassword(user, user.PasswordHashed, password);
                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, Roles[user.Role]),
                    };
                    if (user.EmployeeId != null)
                    {
                        claims.Add(new Claim("EmployeeId", user.EmployeeId.ToString()));
                    }

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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout(bool Logout)
        {
            if (Logout)
            {
                await HttpContext.SignOutAsync
                    (
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles="Administrator")]
        public async Task<IActionResult> Manage()
        {
            var Accounts = context.AppUsers.Include(p => p.Employee).ToList();
            ViewData["Roles"] = Roles;
            return View(Accounts);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> AddAccount(string username, int employeeId, string password, string role, string Status)
        {
            bool finished = false;
            AppUser appUser = new AppUser()
            {
                UserName = username,
                EmployeeId = employeeId,
                Role = Roles.IndexOf(role),
                Active = Status == "on" ? true : false,
            };
            appUser.PasswordHashed = hasher.HashPassword(appUser, password);
            if (ModelState.IsValid)
            {
                try
                {
                    if (await context.AppUsers.FirstOrDefaultAsync(p => p.UserName == username) == null)
                    {
                        context.Add(appUser);
                        await context.SaveChangesAsync();
                        finished = true;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.Employees.Find(appUser.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(finished)
                {
                    return RedirectToAction("Manage");
                }
            }

            var accounts = context.AppUsers.Include(a => a.Employee).ToList();

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            List<Employee> itemsToRemove = new List<Employee>();
            foreach (var item in data)
            {
                if (accounts.FirstOrDefault(p => p.EmployeeId == item.Id) == null)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else
                {
                    itemsToRemove.Add(item);
                }
            }
            ViewBag.Roles = new SelectList(Roles);
            ViewData["EmployeeModel"] = employeeModel;
            ViewData["NoError"] = false;
            List<int> roleIds = new List<int>();
            foreach (var item in data)
            {
                if (!itemsToRemove.Contains(item))
                {
                    roleIds.Add(item.Position);
                }
            }
            return View(roleIds);
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult AddAccount()
        {
            var accounts = context.AppUsers.Include(a => a.Employee).ToList();

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            List<Employee> itemsToRemove = new List<Employee>();
            foreach (var item in data)
            {
                if (accounts.FirstOrDefault(p => p.EmployeeId == item.Id) == null)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else
                {
                    itemsToRemove.Add(item);
                }
            }
            ViewBag.Roles = new SelectList(Roles);
            ViewData["EmployeeModel"] = employeeModel;
            ViewData["NoError"] = true;
            List<int> roleIds = new List<int>();
            foreach (var item in data)
            {
                if(!itemsToRemove.Contains(item))
                {
                    roleIds.Add(item.Position);
                }
            }
            return View(roleIds);
        }
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var account = await context.AppUsers.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        [HttpPost]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> ChangePassword(int Id, string newPassword)
        {
            var account = await context.AppUsers.FindAsync(Id);
            if (account == null)
            {
                return NotFound();
            }
            account.PasswordHashed = hasher.HashPassword(account, newPassword);
            try
            {
                context.Update(account);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Employees.Find(account.Id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Manage");
        }
    }
}
