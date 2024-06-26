using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recrutiment_Test.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }
    [Display(Name = "Employee ID")]
    public int Employee { get; set; }
    [Display(Name = "Absence Reason")]
    public int AbsenceReason { get; set; }
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; }
    [Display(Name = "End Date")]
    public DateOnly EndDate { get; set; }

    public string? Comment { get; set; }

    public int Status { get; set; }

    public virtual ICollection<ApprovalRequest> ApprovalRequests { get; set; } = new List<ApprovalRequest>();

    public virtual Employee EmployeeNavigation { get; set; } = null!;
}
