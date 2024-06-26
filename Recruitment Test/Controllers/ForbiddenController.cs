using Microsoft.AspNetCore.Mvc;

namespace Recrutiment_Test.Controllers
{
    public class ForbiddenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
