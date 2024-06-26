﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? idRange1, int? idRange2, string? subdivision, string? position, bool? status, int? peoplesPartner, int? ooobRange1, int? ooobRange2)
        {
            int? subdivisionId = subdivision == null ? null : subdivisions.IndexOf(subdivision);
            int? positionId = position == null ? null : positions.IndexOf(position);
            idRange1 = idRange1 == null ? 0 : idRange1.Value;
            idRange2 = idRange2 == null ? int.MaxValue : idRange2.Value;
            ooobRange1 = ooobRange1 == null ? 0 : ooobRange1.Value;
            ooobRange2 = ooobRange2 == null ? int.MaxValue : ooobRange2.Value;

            List<Employee> employees = await context.Employees
                .Where(p => search == null || p.FullName.Contains(search))
                .Where(p => p.Id >= idRange1 && p.Id <= idRange2)
                .Where(p => subdivision == null || p.Subdivision == subdivisionId)
                .Where(p => position == null || p.Position == positionId)
                .Where(p => status == null || p.Status == status)
                .Where(p => peoplesPartner == null || p.PeoplePartner == peoplesPartner)
                .Where(p => p.OutOfOfficeBalance >= ooobRange1 && p.OutOfOfficeBalance <= ooobRange2)
                .ToListAsync();

            ViewData["Positions"] = positions;
            ViewData["Subdivisions"] = subdivisions;
            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 0 && item.FullName != "SELF")
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewData["EmployeeModel"] = employeeModel;
            return View(employees);
        }
        [Authorize(Roles = "HR Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee(string fullName, string subdivision, string position, string Status, int peoplesPartner, int oooBalance)
        {
            bool isValid = true;
            Employee employee = new Employee()
            {
                FullName = fullName,
                Subdivision = subdivisions.IndexOf(subdivision),
                Position = positions.IndexOf(position),
                Status = Status == "on" ? true : false,
                PeoplePartner = peoplesPartner,
                OutOfOfficeBalance = oooBalance
            };
            Employee self = await context.Employees.FirstOrDefaultAsync(p => p.FullName == "SELF");

            if (peoplesPartner == self.Id && employee.Position != 0)
            {
                isValid = false;
            }
            if (ModelState.IsValid && isValid)
            {
                try
                {
                    context.Add(employee);
                    await context.SaveChangesAsync();
                    if (employee.PeoplePartner == self.Id)
                    {
                        employee.PeoplePartner = employee.Id;
                    }
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
                if (item.Position == 0 && item.Id != self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else if (item.Id == self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName + " (only for HR Managers)",
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);
            ViewData["EmployeeModel"] = employeeModel;
            return View(false);
        }
        [Authorize(Roles = "HR Manager,Administrator")]
        public async Task<IActionResult> AddEmployee()
        {
            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();
            Employee self = await context.Employees.FirstOrDefaultAsync(p => p.FullName == "SELF");
            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if(item.Position == 0 && item.Id != self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else if (item.Id == self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName + " (only for HR Managers)",
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);
            ViewData["EmployeeModel"] = employeeModel;
            return View(true);
        }
        [Authorize(Roles = "HR Manager,Administrator")]
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
            Employee self = await context.Employees.FirstOrDefaultAsync(p => p.FullName == "SELF");
            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 0 && item.Id != id && item.Id != self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else if (item.Id == self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName + " (only for HR Managers)",
                        Value = item.Id.ToString()
                    });
                }
            }

            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);
            ViewData["EmployeeModel"] = employeeModel;
            return View(Employee);
        }
        [Authorize(Roles = "HR Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string fullName, string subdivision, string position, bool Status, int peoplesPartner, int oooBalance)
        {
            bool isValid = true;
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
            Employee self = await context.Employees.FirstOrDefaultAsync(p => p.FullName == "SELF");
            if (peoplesPartner == self.Id && employee.Position != 0)
            {
                isValid = false;
            }
            if (ModelState.IsValid && isValid)
            {
                try
                {
                    AppUser appUser = await context.AppUsers.FirstOrDefaultAsync(p => p.EmployeeId == id);
                    if (appUser != null)
                    {
                        appUser.Role = employee.Position;
                        context.Update(appUser);
                    }
                    if(employee.PeoplePartner == self.Id)
                    {
                        employee.PeoplePartner = employee.Id;
                    }
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
                if (item.Position == 0 && item.Id != self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
                else if (item.Id == self.Id)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName + " (only for HR Managers)",
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.Position = new SelectList(positions);
            ViewBag.Subdivision = new SelectList(subdivisions);
            ViewData["EmployeeModel"] = employeeModel;
            return View(employee);
        }
        [Authorize(Roles = "HR Manager,Administrator")]
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
        [Authorize(Roles = "HR Manager,Administrator")]
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
        [Authorize(Roles = "Project Manager,Administrator")]
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
        [Authorize(Roles = "Project Manager,Administrator")]
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
        [Authorize(Roles = "Project Manager,Administrator")]
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
