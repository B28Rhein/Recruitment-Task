﻿using Microsoft.AspNetCore.Mvc;
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
            return View();
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
            return View();
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
        public async Task<IActionResult> Deactivate(int ID, bool disactivate)
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
            if (disactivate)
            {
                Employee.Status = false;
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
    }
}
