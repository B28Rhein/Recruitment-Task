using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recrutiment_Test.Models;

public partial class Project
{
    public int Id { get; set; }
    [Display(Name = "Project Type")]
    public int ProjectType { get; set; }
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; }
    [Display(Name = "End Date")]
    public DateOnly? EndDate { get; set; }
    [Display(Name = "Project Manager")]
    public int ProjectManager { get; set; }

    public string? Comment { get; set; }

    public bool Status { get; set; }

    public virtual Employee ProjectManagerNavigation { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
