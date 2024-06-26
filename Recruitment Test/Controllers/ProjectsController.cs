using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        public static List<string> ProjectTypes = new List<string>()
        {
            "Type1",
            "Type2",
            "Type3",
            "Type4"
        };

        private readonly RecruitmentDbContext context;

        public ProjectsController(RecruitmentDbContext context)
        {
            this.context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? idRange1, int? idRange2, string? projectType, DateOnly? startDateRange1, DateOnly? startDateRange2, DateOnly? endDateRange1, DateOnly? endDateRange2, int? projectManager, bool? status)
        {
            idRange1 = idRange1 == null ? 0 : idRange1.Value;
            idRange2 = idRange2 == null ? int.MaxValue : idRange2.Value;
            int? projectTypeId = projectType == null ? null : ProjectTypes.IndexOf(projectType);
            startDateRange1 = startDateRange1 == null ? DateOnly.MinValue : startDateRange1.Value;
            startDateRange2 = startDateRange2 == null ? DateOnly.MaxValue : startDateRange2.Value;
            endDateRange1 = endDateRange1 == null ? DateOnly.MinValue : endDateRange1.Value;
            endDateRange2 = endDateRange2 == null ? DateOnly.MaxValue : endDateRange2.Value;

            EmployeeModel projectManagers = new EmployeeModel();
            projectManagers.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 1)
                {
                    projectManagers.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewData["ProjectManagers"] = projectManagers;
            ViewData["ProjectTypes"] = ProjectTypes;
            ViewBag.ProjectType = new SelectList(ProjectTypes);

            List<Project> project = await context.Projects.Include(p => p.ProjectManagerNavigation).Include(p => p.Employees)
                .Where(p => search == null || p.Id.ToString().Contains(search))
                .Where(p => p.Id >= idRange1 && p.Id <= idRange2)
                .Where(p => projectType == null || p.ProjectType == projectTypeId)
                .Where(p => p.StartDate >= startDateRange1 && p.StartDate <= startDateRange2)
                .Where(p => p.EndDate >= endDateRange1 && p.EndDate <= endDateRange2)
                .Where(p => projectManager == null || p.ProjectManager == projectManager)
                .Where(p => status == null || p.Status == status)
                .ToListAsync();
            return View(project);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> AddProject(string projectType, DateOnly startDate, DateOnly? endDate, int projectManager, string? Comment, string Status)
        {
            Project project = new Project()
            {
                ProjectType = ProjectTypes.IndexOf(projectType),
                StartDate = startDate,
                EndDate = endDate,
                ProjectManager = projectManager,
                Comment = Comment,
                Status = Status == "on" ? true : false,
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Add(project);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.Projects.Find(project.Id) == null)
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

            EmployeeModel projectManagers = new EmployeeModel();
            projectManagers.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 1)
                {
                    projectManagers.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.ProjectTypes = new SelectList(ProjectTypes);
            ViewData["ProjectManagers"] = projectManagers;
            return View(false);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        public IActionResult AddProject()
        {
            EmployeeModel projectManagers = new EmployeeModel();
            projectManagers.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 1)
                {
                    projectManagers.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.ProjectTypes = new SelectList(ProjectTypes);
            ViewData["ProjectManagers"] = projectManagers;
            return View(true);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Project = await context.Projects.FindAsync(id);
            if (Project == null)
            {
                return NotFound();
            }

            EmployeeModel projectManagers = new EmployeeModel();
            projectManagers.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 1)
                {
                    projectManagers.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }

            ViewBag.ProjectTypes = new SelectList(ProjectTypes);
            ViewData["ProjectManagers"] = projectManagers;
            return View(Project);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string projectType, DateOnly startDate, DateOnly? endDate, int projectManager, string? Comment, bool Status)
        {
            Project project = new Project()
            {
                Id = id,
                ProjectType = ProjectTypes.IndexOf(projectType),
                StartDate = startDate,
                EndDate = endDate,
                ProjectManager = projectManager,
                Comment = Comment,
                Status = Status
            };
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(project);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (context.Projects.Find(project.Id) == null)
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

            EmployeeModel projectManagers = new EmployeeModel();
            projectManagers.EmployeeList = new List<SelectListItem>();

            var data = context.Employees.ToList();
            foreach (var item in data)
            {
                if (item.Position == 1)
                {
                    projectManagers.EmployeeList.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }
            }
            ViewBag.ProjectTypes = new SelectList(ProjectTypes);
            ViewData["ProjectManagers"] = projectManagers;
            return View(project);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Project = await context.Projects.FindAsync(id);
            if (Project == null)
            {
                return NotFound();
            }
            return View(Project);
        }
        [Authorize(Roles = "Project Manager,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Deactivate(int ID, bool deactivate)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var Project = await context.Projects.FindAsync(ID);
            if (Project == null)
            {
                return NotFound();
            }
            if (deactivate)
            {
                Project.Status = !Project.Status;
                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Update(Project);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (context.Projects.Find(Project.Id) == null)
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
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Project = await context.Projects.Include(p => p.ProjectManagerNavigation).Include(p => p.Employees).ThenInclude(p => p.PeoplePartnerNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (Project == null)
            {
                return NotFound();
            }
            ViewData["ProjectTypes"] = ProjectTypes;
            return View(Project);
        }
    }
}
