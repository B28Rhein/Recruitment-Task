using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    public class EmployeesController : Controller
    {
        public static List<string> positions = new List<string>()
        {
            "HR Manager",
            "Project Manager",
            "Employee",
            "Administrator"
        };
        public static List<string> subdivisions = new List<string>()
        {
            "subdivision1",
            "subdivision2",
            "subdivision3",
            "Subdivision4"
        };

        private readonly RecruitmentDbContext context;

        public EmployeesController(RecruitmentDbContext context) 
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Positions"] = positions;
            ViewData["Subdivisions"] = subdivisions;
            return View(await context.Employees.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string fullName, string subdivision, string position, string Status, int Id, int oooBalance)
        {
            ViewData["Positions"] = positions;
            ViewData["Subdivisions"] = subdivisions;
            bool Active = Status == "on" ? true : false;
            Employee.AddNewEmployee(fullName, subdivision, position, Active, Id, oooBalance, context);
            return View(await context.Employees.ToListAsync());
        }
        public IActionResult AddEmployee()
        {
            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();
            
            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if(item.Position == 0)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);
            return View(employeeModel);
        }
    }
}
