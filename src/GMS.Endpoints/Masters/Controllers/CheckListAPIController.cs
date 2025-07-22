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
public class CheckListAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckListAPIController> _logger;
    private readonly IMapper _mapper;
    public CheckListAPIController(IUnitOfWork unitOfWork, ILogger<CheckListAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }



    public async Task<IActionResult> CheckInList(CheckListViewModel inputDTO)
    {
        try
        {
            string sQuery = @"Select * from TblCheckLists where isactive=1 and CheckListType=@CheckListType";
            var sParam = new { @CheckListType = inputDTO.CheckListType };
            var res = await _unitOfWork.RoomType.GetTableData<TblCheckListsDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CheckInList)}");
            throw;
        }
    }

    public async Task<IActionResult> CheckListById(int Id)
    {
        try
        {
            string query = "Select * from TblCheckLists where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.RoomType.GetEntityData<TblCheckListsDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CheckListById)}");
            throw;
        }
    }
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            string query = "Select * from TblCheckLists where Id=@Id";
            var param = new { @Id = Id };
            TblCheckLists? dto = await _unitOfWork.TblCheckLists.GetEntityData<TblCheckLists>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.TblCheckLists.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(Delete)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(TblCheckListsDTO dto)
    {
        try
        {
            string eQuery = "Select * from TblCheckLists where Chklist=@Chklist and IsActive=1 and ChecklistType=@ChecklistType";
            var eParam = new { @Chklist = dto.Chklist, @ChecklistType = dto.ChecklistType };
            var exists = await _unitOfWork.TblCheckLists.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Attribute already exists");
            }
            else
            {
                dto.ID = await _unitOfWork.TblCheckLists.AddAsync(_mapper.Map<TblCheckLists>(dto));
                if (dto.ID > 0)
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

    public async Task<IActionResult> Update(TblCheckListsDTO dto)
    {
        try
        {
            string eQuery = "Select * from TblCheckLists where Chklist=@Chklist and IsActive=1 and ChecklistType=@ChecklistType and ID!=@ID";
            var eParam = new { @Chklist = dto.Chklist, @ChecklistType = dto.ChecklistType, @ID = dto.ID };

            var exists = await _unitOfWork.TblCheckLists.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This attribute already exists");
            }
            else
            {
                string query = "Select * from TblCheckLists where Id=@Id";
                var param = new { @Id = dto.ID };
                TblCheckLists? checkLists = await _unitOfWork.TblCheckLists.GetEntityData<TblCheckLists>(query, param);
                if (checkLists != null)
                {
                    checkLists.Chklist = dto.Chklist;
                    checkLists.IsMandatory = dto.IsMandatory;
                    checkLists.ChecklistType = dto.ChecklistType;
                    checkLists.Description = dto.Description;
                    checkLists.Score = dto.Score;

                    var updated = await _unitOfWork.TblCheckLists.UpdateAsync(checkLists);
                    if (updated)
                    {
                        return Ok(checkLists);
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
