﻿using AutoMapper;
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
public class RoomTypeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomTypeAPIController> _logger;
    private readonly IMapper _mapper;
    public RoomTypeAPIController(IUnitOfWork unitOfWork, ILogger<RoomTypeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<IActionResult> List()
    {
        try
        {

            string query = "Select * from RoomType where Status=1 order by roomrank asc";
            var res = await _unitOfWork.RoomType.GetTableData<RoomTypeDTO>(query);
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
    public async Task<IActionResult> RoomTypeById(int Id)
    {
        try
        {
            string query = "Select * from RoomType where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.RoomType.GetEntityData<RoomTypeDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypeById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteRoomType(int Id)
    {
        try
        {
            string query = "Select * from RoomType where Id=@Id";
            var param = new { @Id = Id };
            RoomType? dto = await _unitOfWork.RoomType.GetEntityData<RoomType>(query, param);
            if (dto != null)
            {
                dto.Status = 0;
                var updated = await _unitOfWork.RoomType.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteRoomType)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(RoomTypeDTO dto)
    {
        try
        {
            string eQuery = "Select * from RoomType where Status=@Status and RType=@RType";
            var eParam = new { @Status = 1 , @RType  = dto.Rtype};
            var exists = await _unitOfWork.RoomType.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Room Type already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.RoomType.AddAsync(_mapper.Map<RoomType>(dto));
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

    public async Task<IActionResult> Update(RoomTypeDTO dto)
    {
        try
        {
            string eQuery = "Select * from RoomType where Status=@Status and RType=@RType and Id!=@Id";
            var eParam = new { @Status = 1, @Id = dto.Id, @RType = dto.Rtype };

            var exists = await _unitOfWork.RoomType.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from RoomType where Id=@Id";
                var param = new { @Id = dto.Id };
                RoomType? roomType = await _unitOfWork.RoomType.GetEntityData<RoomType>(query, param);
                if (roomType != null)
                {
                    roomType.Rtype = dto.Rtype;
                    roomType.RoomRank = dto.RoomRank;
                    roomType.Remarks= dto.Remarks;

                    var updated = await _unitOfWork.RoomType.UpdateAsync(roomType);
                    if (updated)
                    {
                        return Ok(roomType);
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
