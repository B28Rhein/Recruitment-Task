using System;
using System.Collections.Generic;

namespace Recrutiment_Test.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public int Employee { get; set; }

    public int AbsenceReason { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Comment { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ApprovalRequest> ApprovalRequests { get; set; } = new List<ApprovalRequest>();

    public virtual Employee EmployeeNavigation { get; set; } = null!;
}
