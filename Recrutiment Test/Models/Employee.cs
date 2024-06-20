using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recrutiment_Test.Models;

public partial class Employee
{
    public int Id { get; set; }
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = null!;

    public int Subdivision { get; set; }

    public int Position { get; set; }
    
    public bool Status { get; set; }
    [Display(Name = "People Partner")]
    public int PeoplePartner { get; set; }
    [Display(Name = "Out-of-Office Balance")]
    public int OutOfOfficeBalance { get; set; }

    public virtual ICollection<ApprovalRequest> ApprovalRequests { get; set; } = new List<ApprovalRequest>();

    public virtual ICollection<Employee> InversePeoplePartnerNavigation { get; set; } = new List<Employee>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual Employee PeoplePartnerNavigation { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public static void AddNewEmployee(string fullName, string subdivision, string position, bool status, int peoplePartner, int Balance, RecruitmentDbContext context)
    {
        Console.WriteLine(fullName);
        context.Employees.Add(new Employee()
        {
            FullName = fullName,
            Subdivision = Controllers.EmployeesController.subdivisions.IndexOf(subdivision),
            Position = Controllers.EmployeesController.positions.IndexOf(position),
            Status = status,
            PeoplePartner = peoplePartner,
            OutOfOfficeBalance = Balance
        });
        context.SaveChanges();
    }
}
