using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.ReviewAndFeedback;
using GMS.Infrastructure.ViewModels.ReviewAndFeedback;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Guests;

[Route("api/[controller]")]
[ApiController]
public class FeedbackAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FeedbackAPIController> _logger;
    private readonly IMapper _mapper;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public FeedbackAPIController(IUnitOfWork unitOfWork, ILogger<FeedbackAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
    }
    public async Task<IActionResult> GetFeedbackAttributes()
    {
        try
        {
            string sQuery = "Select * from Feedback where IsActive=1";
            var feedbackList = await _unitOfWork.Feedback.GetTableData<FeedbackDTO>(sQuery);
            return Ok(feedbackList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetFeedbackAttributes)}");
            throw;
        }
    }
    public async Task<IActionResult> GetFeedbackResultByGuestId(int GuestId, string Level)
    {
        try
        {
            string sQuery = "Select * from FeedbackResults where GuestId=@GuestId and FeedbackType=@FeedbackType";
            var sParam = new { @GuestId = GuestId, @FeedbackType = Level };
            var feedbackList = await _unitOfWork.FeedbackResults.GetTableData<FeedbackResultsDTO>(sQuery, sParam);
            return Ok(feedbackList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetFeedbackAttributes)}");
            throw;
        }
    }
    public async Task<bool> CheckFeedbackEligibility(int GuestId)
    {
        try
        {
            string sQuery = "Select * from FeedbackResults where GuestId=@GuestId and FeedbackType=@FeedbackType";
            var sParam = new { @GuestId = GuestId, @FeedbackType = "Submitted" };
            var feedbackList = await _unitOfWork.FeedbackResults.IsExists(sQuery, sParam);
            return feedbackList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetFeedbackAttributes)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveFeedback(FeedbackViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null && inputDTO.FeedbackResultList != null)
            {
                foreach (var feedbackResult in inputDTO.FeedbackResultList)
                {
                    if (feedbackResult != null)
                    {
                        string sQuery = "Select * from FeedbackResults where FeedbackId=@FeedbackId and guestid=@guestid and FeedbackType='Level2'";
                        var sParam = new { @FeedbackId = feedbackResult.FeedbackId, @guestid = feedbackResult.GuestId };
                        bool isexist = await _unitOfWork.FeedbackResults.IsExists(sQuery, sParam);
                        if (isexist)
                        {
                            var feedbackresultDB = await _unitOfWork.FeedbackResults.GetEntityData<FeedbackResults>(sQuery, sParam);
                            if (feedbackresultDB != null)
                            {
                                feedbackresultDB.Answer = feedbackResult.Answer;
                                await _unitOfWork.FeedbackResults.UpdateAsync(feedbackresultDB);
                            }
                        }
                        else
                        {
                            FeedbackResults feedbackResultsDB = new FeedbackResults();
                            feedbackResultsDB.FeedbackId = feedbackResult.FeedbackId;
                            feedbackResultsDB.Answer = feedbackResult.Answer;
                            feedbackResultsDB.ClientId = feedbackResult.ClientId;
                            feedbackResultsDB.GuestId = feedbackResult.GuestId;
                            feedbackResultsDB.FeedbackDate = DateTime.Now;
                            feedbackResultsDB.FeedbackType = "Level2";
                            await _unitOfWork.FeedbackResults.AddAsync(feedbackResultsDB);
                        }
                    }
                }
            }
            return Ok("Feedback Saved Successfully");
            //string sQuery = "Select * from FeedbackResults where GuestId=@GuestId";
            //var sParam = new { @GuestId = GuestId };
            //var feedbackList = await _unitOfWork.FeedbackResults.GetTableData<FeedbackResultsDTO>(sQuery, sParam);
            //return Ok(feedbackList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetFeedbackAttributes)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveFeedbackOpenText(FeedbackResultsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                string sQuery = "Select * from FeedbackResults where guestid=@guestid and FeedbackType=@FeedbackType";
                var sParam = new { @guestid = inputDTO.GuestId, @FeedbackType = inputDTO.FeedbackType };
                bool isexist = await _unitOfWork.FeedbackResults.IsExists(sQuery, sParam);
                if (isexist)
                {
                    var feedbackresultDB = await _unitOfWork.FeedbackResults.GetEntityData<FeedbackResults>(sQuery, sParam);
                    if (feedbackresultDB != null)
                    {
                        feedbackresultDB.Ans = inputDTO.Ans;
                        var updated = await _unitOfWork.FeedbackResults.UpdateAsync(feedbackresultDB);
                        if (updated)
                            return Ok("Feedback Saved Successfully");
                    }
                }
                else
                {
                    inputDTO.FeedbackDate = DateTime.Now;
                    inputDTO.Id = await _unitOfWork.FeedbackResults.AddAsync(_mapper.Map<FeedbackResults>(inputDTO));
                    if (inputDTO.Id > 0)
                        return Ok("Feedback Saved Successfully");
                }
            }
            return BadRequest("Unable to save feedback");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetFeedbackAttributes)}");
            throw;
        }
    }
}
