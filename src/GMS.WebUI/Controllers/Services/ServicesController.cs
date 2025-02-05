using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Services
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Resources()
        {
            return View();
        }
    }
}
