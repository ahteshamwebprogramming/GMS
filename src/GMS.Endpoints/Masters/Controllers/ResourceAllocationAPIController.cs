using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.ResourceAllocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class ResourceAllocationAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResourceAllocationAPIController> _logger;
    private readonly IMapper _mapper;
    public ResourceAllocationAPIController(IUnitOfWork unitOfWork, ILogger<ResourceAllocationAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetAllSchedules()
    {
        try
        {
            string query = @"Select
                            gs.* 
                            from GuestSchedule gs
                            Left Join TaskMaster tm on gs.TaskId=tm.Id
                            where isnull(tm.Readonly,0)=0
                            order by StartDateTime desc";
            var res = await _unitOfWork.GenOperations.GetTableData<GuestScheduleWithAttributes>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllSchedules)}");
            throw;
        }
    }
    public async Task<IActionResult> ListWithChild()
    {
        try
        {
            string query = @"Select tm.*,rm.RoleName ,cm.Category,tm.Duration,tm.Rate,tm.Remarks,tm.DoctorAdviceRequired
                            from TaskMaster tm 
                            left Join EHRMS.dbo.RoleMaster rm on tm.Department=rm.RoleID 
                            Left Join CategoryMaster cm on tm.CategoryId=cm.Id
                            where tm.IsDeleted=0";
            var res = await _unitOfWork.TaskMaster.GetTableData<TaskMasterWithChild>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ListWithChild)}");
            throw;
        }
    }
    public async Task<IActionResult> TaskMasterById(int Id)
    {
        try
        {
            string query = "Select * from TaskMaster where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.TaskMaster.GetEntityData<TaskMasterDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(TaskMasterById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteTaskMaster(int Id)
    {
        try
        {
            string query = "Select * from TaskMaster where Id=@Id";
            var param = new { @Id = Id };
            TaskMaster? dto = await _unitOfWork.TaskMaster.GetEntityData<TaskMaster>(query, param);
            if (dto != null)
            {
                dto.IsDeleted = true;
                var updated = await _unitOfWork.TaskMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteTaskMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> ManageTaskMasterStatus(TaskMasterDTO inputDto)
    {
        try
        {
            string query = "Select * from TaskMaster where Id=@Id";
            var param = new { @Id = inputDto.Id };
            TaskMaster? dto = await _unitOfWork.TaskMaster.GetEntityData<TaskMaster>(query, param);
            if (dto != null)
            {
                dto.IsActive = inputDto.IsActive;
                var updated = await _unitOfWork.TaskMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteTaskMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> Add(TaskMasterDTO dto)
    {
        try
        {
            //TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            //TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            //dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            //string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and IsActive=1";

            string eQuery = "Select * from TaskMaster where Department=@Department and TaskName=@TaskName and IsDeleted=0";
            //var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime };
            var eParam = new { @Department = dto.Department, @TaskName = dto.TaskName };


            var exists = await _unitOfWork.TaskMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.TaskMaster.AddAsync(_mapper.Map<TaskMaster>(dto));
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

    public async Task<IActionResult> Update(TaskMasterDTO dto)
    {
        try
        {
            //TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            //TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            //dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            //string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and Id!=@Id and isactive=1";
            string eQuery = "Select * from TaskMaster where Department=@Department and TaskName=@TaskName and IsDeleted=0 and Id!=@Id";
            //var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime, @Id = dto.Id };
            var eParam = new { @Department = dto.Department, @Id = dto.Id, @TaskName = dto.TaskName };

            var exists = await _unitOfWork.TaskMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from TaskMaster where IsActive=1 and Id=@Id";
                var param = new { @Id = dto.Id };
                TaskMaster? taskMaster = await _unitOfWork.TaskMaster.GetEntityData<TaskMaster>(query, param);
                if (taskMaster != null)
                {
                    taskMaster.TaskName = dto.TaskName;
                    taskMaster.Department = dto.Department;
                    taskMaster.CategoryId = dto.CategoryId;
                    taskMaster.Duration = dto.Duration;
                    taskMaster.Rate = dto.Rate;
                    taskMaster.DoctorAdviceRequired = dto.DoctorAdviceRequired;
                    taskMaster.Remarks = dto.Remarks;

                    var updated = await _unitOfWork.TaskMaster.UpdateAsync(taskMaster);
                    if (updated)
                    {
                        return Ok(taskMaster);
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
