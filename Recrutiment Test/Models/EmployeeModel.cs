using Microsoft.AspNetCore.Mvc.Rendering;

namespace Recrutiment_Test.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }
}
