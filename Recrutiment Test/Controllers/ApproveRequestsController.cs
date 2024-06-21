using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    public class ApproveRequestsController : Controller
    {
        public static List<string> statuses = new List<string>()
        {
            "New",
            "Approved",
            "Rejected",
        };

        private readonly RecruitmentDbContext context;

        public ApproveRequestsController(RecruitmentDbContext context) 
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = "IDASC";
            return View(await context.ApprovalRequests.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Order)
        {
            Order = Order == null ? "IDASC" : Order;
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = Order;
            List<ApprovalRequest> approvalRequests = await context.ApprovalRequests.ToListAsync();
            switch (Order)
            {
                case "IDASC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "IDDESC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return -X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "APRASC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return X.Approver.CompareTo(Y.Approver);
                    });
                    break;
                case "APRDESC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return -X.Approver.CompareTo(Y.Approver);
                    });
                    break;
                case "LRASC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return X.LeaveRequest.CompareTo(Y.LeaveRequest);
                    });
                    break;
                case "LRDESC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return -X.LeaveRequest.CompareTo(Y.LeaveRequest);
                    });
                    break;
                case "STSASC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return X.Status.CompareTo(Y.Status);
                    });
                    break;
                case "STSDESC":
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return -X.Status.CompareTo(Y.Status);
                    });
                    break;
                default:
                    approvalRequests.Sort(delegate (ApprovalRequest X, ApprovalRequest Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
            }
            return View(approvalRequests);
        }
        //[HttpPost]
        //public async Task<IActionResult> AddApprovalRequest(int Approver, int LeaveRequest, int status, string? Comment)
        //{
        //    ApprovalRequest approvalRequests = new ApprovalRequest()
        //    {
        //        Approver = Approver,
        //        LeaveRequest = LeaveRequest,
        //        Status = status,
        //        Comment = Comment
        //    };
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            context.Add(approvalRequests);
        //            await context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (context.ApprovalRequests.Find(approvalRequests.Id) == null)
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction("Index");
        //    }

        //    EmployeeModel Approvers = new EmployeeModel();
        //    Approvers.EmployeeList = new List<SelectListItem>();

        //    var data = context.Employees.ToList();
        //    foreach (var item in data)
        //    {
        //        if (item.Position == 0)
        //        {
        //            employeeModel.EmployeeList.Add(new SelectListItem
        //            {
        //                Text = item.FullName,
        //                Value = item.Id.ToString()
        //            });
        //        }
        //    }
        //    ViewBag.Position = new SelectList(positions);
        //    ViewBag.Subdivision = new SelectList(subdivisions);
        //    ViewData["EmployeeModel"] = employeeModel;
        //    return View(false);
        //}
        //public IActionResult AddApprovalRequest()
        //{
        //    EmployeeModel employeeModel = new EmployeeModel();
        //    employeeModel.EmployeeList = new List<SelectListItem>();
            
        //    var data = context.ApprovalRequests.ToList();
        //    foreach (var item in data)
        //    {
        //        if(item.Position == 0)
        //        {
        //            employeeModel.EmployeeList.Add(new SelectListItem
        //            {
        //                Text = item.FullName,
        //                Value = item.Id.ToString()
        //            });
        //        }
        //    }
        //    ViewBag.Position = new SelectList(positions);
        //    ViewBag.Subdivision = new SelectList(subdivisions);
        //    ViewData["EmployeeModel"] = employeeModel;
        //    return View(true);
        //}
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.FindAsync(id);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            return View(ApprovalRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int ID, bool disactivate)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.FindAsync(ID);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            if (disactivate)
            {
                ApprovalRequest.Status = 1;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(ApprovalRequest);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.ApprovalRequests.Find(ApprovalRequest.Id) == null)
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
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.FindAsync(id);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            return View(ApprovalRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int ID, bool disactivate)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.FindAsync(ID);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            if (disactivate)
            {
                ApprovalRequest.Status = 2;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(ApprovalRequest);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.ApprovalRequests.Find(ApprovalRequest.Id) == null)
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

            var ApprovalRequest = await context.ApprovalRequests.Include(p => p.ApproverNavigation).Include(p => p.LeaveRequestNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            ViewData["Statuses"] = statuses;

            return View(ApprovalRequest);
        }
    }
}
