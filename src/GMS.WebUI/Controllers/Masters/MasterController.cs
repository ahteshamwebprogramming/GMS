using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class MasterController : Controller
{
    private readonly ILogger<MasterController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;

    public MasterController(ILogger<MasterController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
    }
    public IActionResult Masters()
    {
        return View();
    }
}
