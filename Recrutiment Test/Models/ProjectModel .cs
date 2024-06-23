using Microsoft.AspNetCore.Mvc.Rendering;

namespace Recrutiment_Test.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public List<SelectListItem> ProjectList { get; set; }
    }
}
