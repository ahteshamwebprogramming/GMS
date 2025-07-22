using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class FeedbackAttributesMasterController : Controller
{
    private readonly ILogger<FeedbackAttributesMasterController> _logger;
    private readonly FeedbackAttributesAPIController _feedbackAttributesAPIController;

    public FeedbackAttributesMasterController(ILogger<FeedbackAttributesMasterController> logger, FeedbackAttributesAPIController feedbackAttributesAPIController)
    {
        _logger = logger;
        _feedbackAttributesAPIController = feedbackAttributesAPIController;
    }
    public async Task<IActionResult> List()
    {       
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(FeedbackDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                dataVM.CreationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var res = await _feedbackAttributesAPIController.Add(dataVM);
                return res;
            }
            else
            {
                var res = await _feedbackAttributesAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> ListPartialView()
    {
        FeedbackViewModel dto = new FeedbackViewModel();

        var res = await _feedbackAttributesAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Feedbacks = (List<FeedbackDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_feedbackAttributesMasterList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] FeedbackDTO inputDTO)
    {
        FeedbackViewModel viewModel = new FeedbackViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _feedbackAttributesAPIController.FeedbackById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.Feedback = (FeedbackDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_feedbackAttributesMasterList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] FeedbackDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _feedbackAttributesAPIController.DeleteFeedback(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
