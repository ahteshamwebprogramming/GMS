using Accounting.Controllers;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.ReviewAndFeedback;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.ReviewAndFeedback;
using GMS.WebUI.Controllers.Accounting;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GMS.WebUI.Controllers.ReviewAndFeedbacks;

public class FeedbackController : Controller
{
    private readonly ILogger<FeedbackController> _logger;
    private readonly FeedbackAPIController _feedbackAPIController;
    public FeedbackController(ILogger<FeedbackController> logger, FeedbackAPIController feedbackAPIController)
    {
        _logger = logger;
        _feedbackAPIController = feedbackAPIController;
    }

    [Route("ReviewAndFeedbacks/Feedback/{Id}")]
    public async Task<IActionResult> Feedback(int Id)
    {
        var eligibleForFeedback = await CheckFeedbackEligibility(Id);
        if (eligibleForFeedback != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)eligibleForFeedback).StatusCode == 200)
        {
            FeedbackViewModel dto = new FeedbackViewModel();
            dto.GuestId = Id;
            return View(dto);
        }
        else
        {
            return RedirectToAction("FeedbackReceived", new { Id = Id });
        }
    }
    [Route("ReviewAndFeedbacks/FeedbackLevel2/{Id}")]
    public async Task<IActionResult> FeedbackLevel2(int Id)
    {
        var eligibleForFeedback = await CheckFeedbackEligibility(Id);
        if (eligibleForFeedback != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)eligibleForFeedback).StatusCode == 200)
        {
            FeedbackViewModel dto = new FeedbackViewModel();
            dto.GuestId = Id;
            var feedbackRes = await _feedbackAPIController.GetFeedbackAttributes();
            if (feedbackRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)feedbackRes).StatusCode == 200)
            {
                dto.FeedbackAttributeList = (List<FeedbackDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)feedbackRes).Value;
            }
            dto.FeedbackResultList = await GetFeedbackResult(Id, "Level2");
            return View(dto);
        }
        else
        {
            return RedirectToAction("FeedbackReceived", new { Id = Id });
        }
    }

    public async Task<List<FeedbackResultsDTO>> GetFeedbackResult(int Id, string Level)
    {
        List<FeedbackResultsDTO>? dto = new List<FeedbackResultsDTO>();
        var feedbackResultRes = await _feedbackAPIController.GetFeedbackResultByGuestId(Id, Level);
        if (feedbackResultRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)feedbackResultRes).StatusCode == 200)
        {
            dto = (List<FeedbackResultsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)feedbackResultRes).Value;
        }
        return dto;
    }
    public async Task<IActionResult> CheckFeedbackEligibility(int Id)
    {
        var feedbackResultRes = await _feedbackAPIController.CheckFeedbackEligibility(Id);
        if (feedbackResultRes)
        {
            // Eligible → redirect to feedback page
            return BadRequest("Feedback Already Exists");
        }
        else
        {
            // Not eligible → maybe show a message or return NotFound/BadRequest
            return Ok("Eligible");
        }
    }

    public async Task<IActionResult> SaveFeedbackLevel2([FromBody] FeedbackViewModel inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _feedbackAPIController.SaveFeedback(inputDTO);
            return res;
        }
        return BadRequest("Invalid Request");
    }



    [Route("ReviewAndFeedbacks/FeedbackLevel3/{Id}")]
    public async Task<IActionResult> FeedbackLevel3(int Id)
    {
        var eligibleForFeedback = await CheckFeedbackEligibility(Id);
        if (eligibleForFeedback != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)eligibleForFeedback).StatusCode == 200)
        {
            FeedbackViewModel dto = new FeedbackViewModel();
            dto.FeedbackResultList = await GetFeedbackResult(Id, "Level3");
            dto.GuestId = Id;
            return View(dto);
        }
        else
        {
            return RedirectToAction("FeedbackReceived", new { Id = Id });
        }
    }

    public async Task<IActionResult> SaveFeedbackOpenText([FromBody] FeedbackResultsDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _feedbackAPIController.SaveFeedbackOpenText(inputDTO);
            return res;
        }
        return BadRequest("Invalid Request");
    }

    [Route("ReviewAndFeedbacks/FeedbackLevel4/{Id}")]
    public async Task<IActionResult> FeedbackLevel4(int Id)
    {
        var eligibleForFeedback = await CheckFeedbackEligibility(Id);
        if (eligibleForFeedback != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)eligibleForFeedback).StatusCode == 200)
        {
            FeedbackViewModel dto = new FeedbackViewModel();
            dto.FeedbackResultList = await GetFeedbackResult(Id, "Level4");
            dto.GuestId = Id;
            return View(dto);
        }
        else
        {
            return RedirectToAction("FeedbackReceived", new { Id = Id });
        }
    }
    [Route("ReviewAndFeedbacks/FeedbackThanks/{Id}")]
    public async Task<IActionResult> FeedbackThanks(int Id)
    {
        var eligibleForFeedback = await CheckFeedbackEligibility(Id);
        if (eligibleForFeedback != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)eligibleForFeedback).StatusCode == 200)
        {
            FeedbackViewModel dto = new FeedbackViewModel();
            dto.GuestId = Id;
            return View(dto);
        }
        else
        {
            return RedirectToAction("FeedbackReceived", new { Id = Id });
        }
    }
    [Route("ReviewAndFeedbacks/FeedbackReceived/{Id}")]
    public IActionResult FeedbackReceived(int Id)
    {
        FeedbackViewModel dto = new FeedbackViewModel();
        dto.GuestId = Id;
        return View(dto);
    }
}
