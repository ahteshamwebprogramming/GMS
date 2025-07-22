using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class FeedbackAttributesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FeedbackAttributesAPIController> _logger;
    private readonly IMapper _mapper;
    public FeedbackAttributesAPIController(IUnitOfWork unitOfWork, ILogger<FeedbackAttributesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from Feedback where IsActive=1";
            var res = await _unitOfWork.Feedback.GetTableData<FeedbackDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    //public async Task<IActionResult> ListWithChild()
    //{
    //    try
    //    {

    //        string query = "Select tm.*,rm.RoleName from TaskMaster tm left Join EHRMS.dbo.RoleMaster rm on tm.Department=rm.RoleID where tm.IsDeleted=0";
    //        var res = await _unitOfWork.TaskMaster.GetTableData<TaskMasterWithChild>(query);
    //        return Ok(res);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
    //        throw;
    //    }
    //}
    public async Task<IActionResult> FeedbackById(int Id)
    {
        try
        {
            string query = "Select * from Feedback where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.Feedback.GetEntityData<FeedbackDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FeedbackById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteFeedback(int Id)
    {
        try
        {
            string query = "Select * from Feedback where Id=@Id";
            var param = new { @Id = Id };
            Feedback? dto = await _unitOfWork.Feedback.GetEntityData<Feedback>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.Feedback.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteFeedback)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(FeedbackDTO dto)
    {
        try
        {
            string eQuery = "Select * from Feedback where IsActive=@IsActive and Question=@Question";
            var eParam = new { @IsActive = 1, @Question = dto.Question };
            var exists = await _unitOfWork.Feedback.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Category already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.Feedback.AddAsync(_mapper.Map<Feedback>(dto));
                if (dto.Id > 0)
                {
                    return Ok(dto);
                }
                else
                {
                    return BadRequest("Unable to add right now");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(Add)}");
            throw;
        }
    }

    public async Task<IActionResult> Update(FeedbackDTO dto)
    {
        try
        {
            string eQuery = "Select * from Feedback where IsActive=@IsActive and Question=@Question and Id!=@Id";
            var eParam = new { @IsActive = 1, @Id = dto.Id, @FeedbackName = dto.Question };

            var exists = await _unitOfWork.Feedback.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from Feedback where Id=@Id";
                var param = new { @Id = dto.Id };
                Feedback? Feedback = await _unitOfWork.Feedback.GetEntityData<Feedback>(query, param);
                if (Feedback != null)
                {
                    Feedback.Question = dto.Question;                   

                    var updated = await _unitOfWork.Feedback.UpdateAsync(Feedback);
                    if (updated)
                    {
                        return Ok(Feedback);
                    }
                }
                return BadRequest("Unable to update right now");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(Update)}");
            throw;
        }
    }
}
