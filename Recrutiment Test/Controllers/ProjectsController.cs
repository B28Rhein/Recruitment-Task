using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recrutiment_Test.Models;
using System.Text.Encodings.Web;

namespace Recrutiment_Test.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            ViewData["ProjectType"] = ProjectTypes;
            ViewData["SortOrder"] = "IDASC";
            return View(await context.Projects.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Order)
        {
            Order = Order == null ? "IDASC" : Order;
            ViewData["ProjectTypes"] = ProjectTypes;
            ViewData["SortOrder"] = Order;
            
            List<Project> project = await context.Projects.Include(p => p.ProjectManagerNavigation).ToListAsync();
            switch (Order)
            {
                case "IDASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "IDDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return -X.Id.CompareTo(Y.Id);
                    });
                    break;
                case "PTASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.ProjectType.CompareTo(Y.ProjectType);
                    });
                    break;
                case "PTDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return -X.ProjectType.CompareTo(Y.ProjectType);
                    });
                    break;
                case "SDASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.StartDate.CompareTo(Y.StartDate);
                    });
                    break;
                case "SDDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return -X.StartDate.CompareTo(Y.StartDate);
                    });
                    break;
                case "EDASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        if (!(X.EndDate == null || Y.EndDate == null))
                            return X.EndDate.Value.CompareTo(Y.EndDate.Value);
                        else if (X.EndDate != null)
                            return 1;
                        else if (Y.EndDate != null)
                            return -1;
                        else return 0;
                    });
                    break;
                case "EDDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        if (!(X.EndDate == null || Y.EndDate == null))
                            return -X.EndDate.Value.CompareTo(Y.EndDate.Value);
                        else if (X.EndDate != null)
                            return -1;
                        else if (Y.EndDate != null)
                            return 1;
                        else return 0;
                    });
                    break;
                case "PMASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.ProjectManager.CompareTo(Y.ProjectManager);
                    });
                    break;
                case "PMDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return -X.ProjectManager.CompareTo(Y.ProjectManager);

                    });
                    break;
                case "STSASC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.Status.CompareTo(Y.Status);
                    });
                    break;
                case "STSDESC":
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return -X.Status.CompareTo(Y.Status);
                    });
                    break;
                default:
                    project.Sort(delegate (Project X, Project Y)
                    {
                        return X.Id.CompareTo(Y.Id);
                    });
                    break;
            }
            return View(project);
        }
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Project = await context.Projects.Include(p => p.ProjectManagerNavigation).FirstOrDefaultAsync(p => p.Id == id);
            if (Project == null)
            {
                return NotFound();
            }
            ViewData["ProjectTypes"] = ProjectTypes;
            return View(Project);
        }
    }
}
