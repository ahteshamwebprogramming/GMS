using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Admin.Actions;
using Guests.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GMS.WebUI.Controllers.Guests;

public class AdminActionsController : Controller
{
    private readonly ILogger<AdminActionsController> _logger;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    private readonly AdminActionsAPIController _adminActionsAPIController;

    public AdminActionsController(ILogger<AdminActionsController> logger, IWebHostEnvironment hostingEnv, AdminActionsAPIController adminActionsAPIController)
    {
        _logger = logger;
        _hostingEnv = hostingEnv;
        _adminActionsAPIController = adminActionsAPIController;
    }
    public IActionResult GuestActions()
    {
        return View();
    }

    public async Task<IActionResult> SearchGuests([FromBody] GuestsActionViewModel inputDTO)
    {
        inputDTO.GuestsList = new List<MembersDetailsDTO>();

        var res = await _adminActionsAPIController.SearchGuests(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<MembersDetailsDTO>;
            if (data != null)
            {
                inputDTO.GuestsList = data;
            }
        }
        return PartialView("_guestActions/_searchResultGuests", inputDTO);
    }

    public async Task<IActionResult> GetGuestDetailsByID([FromBody] GuestsActionViewModel inputDTO)
    {
        inputDTO.GuestsList = new List<MembersDetailsDTO>();

        var res = await _adminActionsAPIController.SearchGuestsById(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<MembersDetailsDTO>;
            if (data != null)
            {
                inputDTO.GuestsList = data;
            }
        }
        return PartialView("_guestActions/_memberDetailsEditMode", inputDTO);
    }
    public async Task<IActionResult> GetRoomAlocationByGuestID([FromBody] GuestsActionViewModel inputDTO)
    {
        var res = await _adminActionsAPIController.SearchRoomAllocationByGuestId(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<RoomAllocationDTO>;
            if (data != null)
            {
                inputDTO.RoomAllocationList = data;
            }
        }
        return PartialView("_guestActions/_searchResultGuests", inputDTO);
    }

    public async Task<IActionResult> GetBillingByGuestID([FromBody] GuestsActionViewModel inputDTO)
    {
        var res = await _adminActionsAPIController.SearchBillingByGuestId(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<BillingDTO>;
            if (data != null)
            {
                inputDTO.BillingList = data;
            }
        }
        return PartialView("_guestActions/_searchResultGuests", inputDTO);
    }
    public async Task<IActionResult> GetPaymentByGuestID([FromBody] GuestsActionViewModel inputDTO)
    {
        inputDTO.GuestsList = new List<MembersDetailsDTO>();

        var res = await _adminActionsAPIController.SearchGuests(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<PaymentDTO>;
            if (data != null)
            {
                inputDTO.PaymentList = data;
            }
        }
        return PartialView("_guestActions/_searchResultGuests", inputDTO);
    }
    public async Task<IActionResult> GetSettlementByGuestID([FromBody] GuestsActionViewModel inputDTO)
    {
        inputDTO.GuestsList = new List<MembersDetailsDTO>();

        var res = await _adminActionsAPIController.SearchGuests(inputDTO);
        if (res is OkObjectResult okResult)
        {
            var data = okResult.Value as List<SettlementDTO>;
            if (data != null)
            {
                inputDTO.SettlementList = data;
            }
        }
        return PartialView("_guestActions/_searchResultGuests", inputDTO);
    }
}
