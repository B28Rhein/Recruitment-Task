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
            ViewData["SortOrder"] = "IDASC";
            return View(await context.Employees.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Order)
        {
            Order = Order == null ? "IDASC" : Order;
            ViewData["Positions"] = positions;
            ViewData["Subdivisions"] = subdivisions;
            ViewData["SortOrder"] = Order;
            List<Employee> employees = await context.Employees.ToListAsync();
            switch (Order)
            {
                case "IDASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "IDDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "NAMEASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.FullName.CompareTo(Y.FullName);
                    });
                    break;
                case "NAMEDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.FullName.CompareTo(Y.FullName);
                    });
                    break;
                case "SDASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.Subdivision.CompareTo(Y.Subdivision);
                    });
                    break;
                case "SDDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.Subdivision.CompareTo(Y.Subdivision);
                    });
                    break;
                case "POSASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.Position.CompareTo(Y.Position);
                    });
                    break;
                case "POSDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.Position.CompareTo(Y.Position);
                    });
                    break;
                case "STSASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.Status.CompareTo(Y.Status);
                    });
                    break;
                case "STSDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.Status.CompareTo(Y.Status);
                    });
                    break;
                case "PPASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.PeoplePartner.CompareTo(Y.PeoplePartner);
                    });
                    break;
                case "PPDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.PeoplePartner.CompareTo(Y.PeoplePartner);

                    });
                    break;
                case "OOOBASC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.OutOfOfficeBalance.CompareTo(Y.OutOfOfficeBalance);
                    });
                    break;
                case "OOOBDESC":
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return -X.OutOfOfficeBalance.CompareTo(Y.OutOfOfficeBalance);
                    });
                    break;
                default:
                    employees.Sort(delegate (Employee X, Employee Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
            }
            return View(employees);
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(string fullName, string subdivision, string position, string Status, int peoplesPartner, int oooBalance)
        {
            Employee employee = new Employee()
            {
                FullName = fullName,
                Subdivision = subdivisions.IndexOf(subdivision),
                Position = positions.IndexOf(position),
                Status = Status == "on" ? true : false,
                PeoplePartner = peoplesPartner,
                OutOfOfficeBalance = oooBalance
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Add(employee);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.Employees.Find(employee.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 0)
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
            ViewData["EmployeeModel"] = employeeModel;
            return View(false);
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
            ViewData["EmployeeModel"] = employeeModel;
            return View(true);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 0 && item.Id != id)
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
            ViewData["EmployeeModel"] = employeeModel;
            return View(Employee);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string fullName, string subdivision, string position, bool Status, int peoplesPartner, int oooBalance)
        {
            Employee employee = new Employee()
            {
                Id = id,
                FullName = fullName,
                Subdivision = subdivisions.IndexOf(subdivision),
                Position = positions.IndexOf(position),
                Status = Status,
                PeoplePartner = peoplesPartner,
                OutOfOfficeBalance = oooBalance
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(employee);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.Employees.Find(employee.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 0)
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
            ViewData["EmployeeModel"] = employeeModel;
            return View(employee);
        }
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }
            return View(Employee);
        }
        [HttpPost]
        public async Task<IActionResult> Deactivate(int ID, bool deactivate)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var Employee = await context.Employees.FindAsync(ID);
            if (Employee == null)
            {
                return NotFound();
            }
            if (deactivate)
            {
                Employee.Status = !Employee.Status;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(Employee);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.Employees.Find(Employee.Id) == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await context.Employees.Include(p => p.PeoplePartnerNavigation).Include(p => p.Projects).ThenInclude(p => p.ProjectManagerNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (Employee == null)
            {
                return NotFound();
            }
            ViewData["Positions"] = positions;
            ViewData["Subdivisions"] = subdivisions;

            return View(Employee);
        }
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await context.Employees.Include(p => p.Projects).FirstOrDefaultAsync(p => p.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            ProjectModel Projects = new ProjectModel();
            Projects.ProjectList = new List<SelectListItem>();

            var data = await context.Projects.ToListAsync();
            foreach (var item in data)
            {
                if (employee.Projects.Count == 0 || employee.Projects.FirstOrDefault(p => p.Id == item.Id) == null)
                {
                    Projects.ProjectList.Add(new SelectListItem
                    {
                        Text = $"Project id: {item.Id}",
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewData["ProjectModel"] = Projects;
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Assign(int ID, bool assign, int project)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var employee = await context.Employees.Include(p => p.ProjectsNavigation).FirstOrDefaultAsync(p => p.Id == ID);
            if (employee == null)
            {
                return NotFound();
            }
            if (assign)
            {
                if (ModelState.IsValid)
                {
                    employee.Projects.Add(await context.Projects.FindAsync(project));
                    try
                    {
                        context.Update(employee);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.LeaveRequests.Find(employee.Id) == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
