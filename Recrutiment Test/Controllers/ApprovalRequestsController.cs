using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
    public class ApprovalRequestsController : Controller
    {
        public static List<string> statuses = new List<string>()
        {
            "New",
            "Approved",
            "Rejected",
            "Cancelled"
        };

        private readonly RecruitmentDbContext context;

        public ApprovalRequestsController(RecruitmentDbContext context) 
        {
            this.context = context;
        }
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        public async Task<IActionResult> Index()
        {
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = "IDASC";
            return View(await context.ApprovalRequests.ToListAsync());
        }
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        [HttpGet]
        public async Task<IActionResult> Index(string Order)
        {
            Order = Order == null ? "IDASC" : Order;
            ViewData["Statuses"] = statuses;
            ViewData["SortOrder"] = Order;
            List<ApprovalRequest> approvalRequests = await context.ApprovalRequests.Include(p => p.ApproverNavigation).Include(p => p.LeaveRequestNavigation).ToListAsync();
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Approve(int ID, bool approve)
        {
            if (ID == null)
            {
                return NotFound();
            }
            bool approved = true;
            var ApprovalRequest = await context.ApprovalRequests.Include(p => p.LeaveRequestNavigation).ThenInclude(p => p.EmployeeNavigation).Include(p => p.LeaveRequestNavigation).ThenInclude(p => p.ApprovalRequests).FirstOrDefaultAsync(p => p.Id == ID);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            if (approve)
            {
                ApprovalRequest.Status = 1;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(ApprovalRequest);
                        LeaveRequest leaveRequest = ApprovalRequest.LeaveRequestNavigation;
                        foreach(var approval in leaveRequest.ApprovalRequests)
                        {
                            if(approval.Status != 1)
                            {
                                approved = false;
                            }
                        }
                        if(approved)
                        {
                            leaveRequest.Status = 1;
                            context.Update(leaveRequest);
                            int ammountOfDays = ((int)(leaveRequest.EndDate.ToDateTime(new TimeOnly(0, 0)) - leaveRequest.StartDate.ToDateTime(new TimeOnly(0))).TotalDays);
                            Employee employee = leaveRequest.EmployeeNavigation;
                            employee.OutOfOfficeBalance -= ammountOfDays;
                            context.Update(employee);
                        }
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Reject(int ID, bool reject, string comment)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.Include(p => p.LeaveRequestNavigation).ThenInclude(p => p.ApprovalRequests).Include(p => p.ApproverNavigation).FirstOrDefaultAsync(p => p.Id == ID);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            if (reject)
            {
                ApprovalRequest.Status = 2;
                if (ModelState.IsValid)
                {
                    try
                    {
                        LeaveRequest leaveRequest = ApprovalRequest.LeaveRequestNavigation;
                        leaveRequest.Status = 2;
                        string Comment = leaveRequest.Comment == null ? Comment = $"<br />Request rejected by {ApprovalRequest.ApproverNavigation.FullName} ({ApprovalRequest.ApproverNavigation.Id}) with comment <br />" + comment : Comment = leaveRequest.Comment + $"<br />Request rejected by {ApprovalRequest.ApproverNavigation.FullName} ({ApprovalRequest.ApproverNavigation.Id}) with comment: <br />" + comment;
                        for (int i = 0; i < leaveRequest.ApprovalRequests.Count; i++) 
                        {
                            leaveRequest.ApprovalRequests.ToList()[i].Comment = Comment;
                            context.Update(leaveRequest.ApprovalRequests.ToList()[i]);
                        }
                        leaveRequest.Comment = Comment;
                        context.Update(leaveRequest);
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
        [Authorize(Roles = "HR Manager,Project Manager,Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ApprovalRequest = await context.ApprovalRequests.Include(p => p.ApproverNavigation).Include(p => p.LeaveRequestNavigation).ThenInclude(p => p.EmployeeNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (ApprovalRequest == null)
            {
                return NotFound();
            }
            ViewData["Statuses"] = statuses;

            return View(ApprovalRequest);
        }
    }
}
