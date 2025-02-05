using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.Guests;
using GMS.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GMS.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GuestsAPIController _guestsAPIController;
        private readonly AccountsAPIController _accountsAPIController;

        public HomeController(ILogger<HomeController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController)
        {
            _logger = logger;
            _guestsAPIController = guestsAPIController;
            _accountsAPIController = accountsAPIController;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Index1()
        {
            var res = await _guestsAPIController.GetGuestsList();

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                List<GMSFinalGuestDTO>? dto = (List<GMSFinalGuestDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return View(dto);
            }
            return View();
        }
        public async Task<IActionResult> EHRMSLogin()
        {
            var res = await _accountsAPIController.GetLoginList();

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                List<EHRMSLoginDTO>? dto = (List<EHRMSLoginDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return View(dto);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SendFeedback()
        {
            return View();
        }
        public IActionResult ReviewFeedback()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
