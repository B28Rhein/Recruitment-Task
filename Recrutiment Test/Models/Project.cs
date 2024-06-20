using System;
using System.Collections.Generic;

namespace Recrutiment_Test.Models;

public partial class Project
{
    public int Id { get; set; }

    public int? ProjectType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? ProjectManager { get; set; }

    public string Comment { get; set; } = null!;

    public int? Status { get; set; }

    public virtual Employee? ProjectManagerNavigation { get; set; }
}
