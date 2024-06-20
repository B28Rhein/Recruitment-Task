using Microsoft.AspNetCore.Mvc;

namespace Recrutiment_Test.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
