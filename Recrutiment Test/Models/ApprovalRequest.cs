using System;
using System.Collections.Generic;

namespace Recrutiment_Test.Models;

public partial class ApprovalRequest
{
    public int Id { get; set; }

    public int Approver { get; set; }

    public int LeaveRequest { get; set; }

    public int Status { get; set; }

    public string? Comment { get; set; }

    public virtual Employee ApproverNavigation { get; set; } = null!;

    public virtual LeaveRequest LeaveRequestNavigation { get; set; } = null!;
}
