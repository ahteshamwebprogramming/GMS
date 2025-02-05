using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.WebUI.Controllers.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Guests;

public class ServicesController : Controller
{
    private readonly ILogger<ServicesController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly MasterScheduleAPIController _masterScheduleAPIController;

    public ServicesController(ILogger<ServicesController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, MasterScheduleAPIController masterScheduleAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _masterScheduleAPIController = masterScheduleAPIController;
    }
    public IActionResult AddServices()
    {
        return View();
    }
}
