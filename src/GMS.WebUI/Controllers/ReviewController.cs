using GMS.Endpoints.Guests;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers;

public class ReviewController : Controller
{
    private readonly ILogger<ReviewController> _logger;
    private readonly GuestsAPIController _guestsAPIController;

    public ReviewController(ILogger<ReviewController> logger, GuestsAPIController guestsAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
    }

    [Authorize]
    public IActionResult ReviewList()
    {
        return View();
    }

    public async Task<IActionResult> ReviewListGridViewPartialView([FromBody] GuestsGridViewParameters inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();
        viewModel = await ReviewDataForGridViewPartialView(inputDTO);
        return PartialView("_reviewList/_reviewGuestCards", viewModel);
    }

    public async Task<GuestsListViewModel> ReviewDataForGridViewPartialView([FromBody] GuestsGridViewParameters inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();
        var res = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Data");
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }

        var resCount = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Count");
        if (resCount != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).StatusCode == 200)
        {
            GuestsGridViewParameters pageDetails = new GuestsGridViewParameters();
            var totalRecords = (List<int>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).Value;
            pageDetails.TotalRecords = totalRecords == null ? 0 : totalRecords.Count == 0 ? 0 : totalRecords[0];

            pageDetails.PageSize = (inputDTO != null && inputDTO.PageSize != null) ? inputDTO.PageSize : 10;
            pageDetails.PageNumber = (inputDTO != null && inputDTO.PageNumber != null) ? inputDTO.PageNumber : 1;
            pageDetails.TotalPages = (int)Math.Ceiling((double?)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));
            pageDetails.SearchKeyword = inputDTO == null ? "" : String.IsNullOrEmpty(inputDTO.SearchKeyword) ? "" : inputDTO.SearchKeyword;
            viewModel.GuestsGridViewParameters = pageDetails;
        }

        return viewModel;
    }

    [Route("/Review/RegistrationInfo/{GuestId}")]
    [Authorize]
    public async Task<IActionResult> RegistrationInfo(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.GuestId = GuestId;
            return View(membersDetailsDTOs);
        }
    }
}

