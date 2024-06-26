using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Diagnostics;

namespace Recrutiment_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RecruitmentDbContext context;

        public HomeController(ILogger<HomeController> logger, RecruitmentDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            if(User.Claims.ToArray().Length > 2)
            {
                var user = await context.AppUsers.FirstOrDefaultAsync(p => p.UserName == User.Identity.Name);
                var employee = await context.Employees.FirstOrDefaultAsync(p => p.Id == user.EmployeeId);
                return View(employee);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
