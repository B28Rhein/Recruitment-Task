using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    public class LeaveRequestsController : Controller
    {
        public static List<string> absenceReasons = new List<string>()
        {
            "reason1",
            "reason2",
            "reason3",
            "reason4"
        };
        public static List<string> statuses = new List<string>()
        {
            "New",
            "Approved",
            "Rejected",
            "Submited",
            "Canceled"
        };

        private readonly RecruitmentDbContext context;

        public LeaveRequestsController(RecruitmentDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["AbsenceReasons"] = absenceReasons;
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = "IDASC";
            return View(await context.LeaveRequests.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Order)
        {
            Order = Order == null ? "IDASC" : Order;
            ViewData["AbsenceReasons"] = absenceReasons;
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = Order;
            List<LeaveRequest> leaveRequests = await context.LeaveRequests.Include(p => p.EmployeeNavigation).ToListAsync();
            switch (Order)
            {
                case "IDASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "IDDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "EMPASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.Employee.CompareTo(Y.Employee);
                    });
                    break;
                case "EMPDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.Employee.CompareTo(Y.Employee);
                    });
                    break;
                case "ARASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.AbsenceReason.CompareTo(Y.AbsenceReason);
                    });
                    break;
                case "ARDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.AbsenceReason.CompareTo(Y.AbsenceReason);
                    });
                    break;
                case "SDASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.StartDate.CompareTo(Y.StartDate);
                    });
                    break;
                case "SDDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.StartDate.CompareTo(Y.StartDate);
                    });
                    break;
                case "EDASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.EndDate.CompareTo(Y.EndDate);
                    });
                    break;
                case "EDDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.EndDate.CompareTo(Y.EndDate);

                    });
                    break;
                case "STSASC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.Status.CompareTo(Y.Status);
                    });
                    break;
                case "STSDESC":
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return -X.Status.CompareTo(Y.Status);
                    });
                    break;
                default:
                    leaveRequests.Sort(delegate (LeaveRequest X, LeaveRequest Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
            }
            return View(leaveRequests);
        }
        [HttpPost]
        public async Task<IActionResult> AddLeaveRequest(int employee, string absenceReason, DateOnly startDate, DateOnly endDate, string? Comment, int Status)
        {
            LeaveRequest leaveRequest = new LeaveRequest()
            {
                Employee = employee, 
                AbsenceReason = absenceReasons.IndexOf(absenceReason),
                StartDate = startDate,
                EndDate = endDate,
                Comment = Comment,
                Status = Status
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Add(leaveRequest);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.LeaveRequests.Find(leaveRequest.Id) == null)
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
                if (item.Position == 2)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.AbsenceReasons = new SelectList(absenceReasons);
            ViewBag.Statuses = new SelectList(statuses);
            ViewData["EmployeeModel"] = employeeModel;
            return View(false);
        }
        public IActionResult AddLeaveRequest()
        {
            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 2)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.AbsenceReasons = new SelectList(absenceReasons);
            ViewBag.Statuses = new SelectList(statuses);
            ViewData["EmployeeModel"] = employeeModel;
            return View(true);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveRequest = await context.LeaveRequests.FindAsync(id);
            if (LeaveRequest == null)
            {
                return NotFound();
            }

            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 2)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }

            ViewBag.AbsenceReasons = new SelectList(absenceReasons);
            ViewBag.Statuses = new SelectList(statuses);
            ViewData["EmployeeModel"] = employeeModel;
            return View(LeaveRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, int employee, string absenceReason, DateOnly startDate, DateOnly endDate, string? Comment, int Status)
        {
            LeaveRequest leaveRequest = new LeaveRequest()
            {
                Id = id,
                Employee = employee,
                AbsenceReason = absenceReason.IndexOf(absenceReason),
                StartDate = startDate,
                EndDate = endDate,
                Comment = Comment,
                Status = Status
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(leaveRequest);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.LeaveRequests.Find(leaveRequest.Id) == null)
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
                if (item.Position == 2)
                {
                    employeeModel.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.AbsenceReasons = new SelectList(absenceReasons);
            ViewBag.Statuses = new SelectList(statuses);
            ViewData["EmployeeModel"] = employeeModel;
            return View(leaveRequest);
        }
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveRequest = await context.LeaveRequests.FindAsync(id);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            return View(LeaveRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Submit(int ID, bool submit)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var LeaveRequest = await context.LeaveRequests.FindAsync(ID);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            if (submit)
            {
                LeaveRequest.Status = 1;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(LeaveRequest);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.LeaveRequests.Find(LeaveRequest.Id) == null)
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
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveRequest = await context.LeaveRequests.FindAsync(id);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            return View(LeaveRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int ID, bool cancel)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var LeaveRequest = await context.LeaveRequests.FindAsync(ID);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            if (cancel)
            {
                LeaveRequest.Status = 2;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(LeaveRequest);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.LeaveRequests.Find(LeaveRequest.Id) == null)
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

            var LeaveRequest = await context.LeaveRequests.Include(p => p.EmployeeNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            ViewData["AbsenceReasons"] = absenceReasons;
            ViewData["Statuses"] = statuses;

            return View(LeaveRequest);
        }
    }
}
