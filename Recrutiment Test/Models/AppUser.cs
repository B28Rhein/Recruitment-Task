using System;
using System.Collections.Generic;

namespace Recrutiment_Test.Models;

public partial class AppUser
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHashed { get; set; } = null!;

    public int Role { get; set; }

    public bool Active { get; set; }

    public virtual Employee? Employee { get; set; }
}
