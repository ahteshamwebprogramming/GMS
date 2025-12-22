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
    private readonly IConfiguration _configuration;
    public ResourceAllocationAPIController(IUnitOfWork unitOfWork, ILogger<ResourceAllocationAPIController> logger, IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
    }

    private string GetEHRMSDatabaseName()
    {
        var connectionString = _configuration.GetConnectionString("EHRMSConnectionDB");
        if (string.IsNullOrEmpty(connectionString))
        {
            return "EHRMS"; // Fallback to default
        }

        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.Trim().StartsWith("Initial Catalog", StringComparison.OrdinalIgnoreCase) ||
                part.Trim().StartsWith("Database", StringComparison.OrdinalIgnoreCase))
            {
                var dbName = part.Split('=')[1]?.Trim();
                return dbName ?? "EHRMS";
            }
        }

        return "EHRMS"; // Fallback to default
    }
    public async Task<IActionResult> GetAllSchedules()
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();
            string query = $@"Select
                            gs.Id,
                            gs.GuestId,
                            gs.StartDateTime,
                            gs.EndDateTime,
                            gs.Duration,
                            gs.TaskId,
                            gs.EmployeeId1,
                            gs.EmployeeId2,
                            gs.EmployeeId3,
                            gs.SessionId,
                            gs.ResourceId,
                            rm.ResourceName,
                            wm1.WorkerName Therapist1Name,
                            wm2.WorkerName Therapist2Name,
                            wm3.WorkerName Therapist3Name,
                            md.CustomerName GuestName,
                            ra.Rnumber RoomNo
                            from GuestSchedule gs
                            Left Join TaskMaster tm on gs.TaskId=tm.Id
                            Left Join ResourceMaster rm on gs.ResourceId=rm.Id
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm1 on gs.EmployeeId1=wm1.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm2 on gs.EmployeeId2=wm2.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm3 on gs.EmployeeId3=wm3.WorkerID
                            Left Join MembersDetails md on gs.GuestId=md.Id
                            Left Join RoomAllocation ra on gs.GuestId=ra.GuestID
                            where CAST(gs.StartDateTime AS DATE) >= CAST(GETDATE() AS DATE)
                            AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                            AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)
                            order by gs.StartDateTime asc";
            var res = await _unitOfWork.GenOperations.GetTableData<GuestScheduleWithAttributes>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllSchedules)}");
            throw;
        }
    }

    public async Task<IActionResult> GetScheduleById(int Id)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();
            string query = $@"Select
                            gs.*,
                            rm.ResourceName,
                            wm1.WorkerName Therapist1Name,
                            wm2.WorkerName Therapist2Name,
                            wm3.WorkerName Therapist3Name,
                            md.CustomerName GuestName,
                            ra.Rnumber RoomNo
                            from GuestSchedule gs
                            Left Join TaskMaster tm on gs.TaskId=tm.Id
                            Left Join ResourceMaster rm on gs.ResourceId=rm.Id
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm1 on gs.EmployeeId1=wm1.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm2 on gs.EmployeeId2=wm2.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm3 on gs.EmployeeId3=wm3.WorkerID
                            Left Join MembersDetails md on gs.GuestId=md.Id
                            Left Join RoomAllocation ra on gs.GuestId=ra.GuestID
                            where gs.Id=@Id
                            AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                            AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)";
            var param = new { @Id = Id };
            var res = await _unitOfWork.GenOperations.GetEntityData<GuestScheduleWithAttributes>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving schedule {nameof(GetScheduleById)}");
            throw;
        }
    }
    public async Task<IActionResult> ListWithChild()
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();
            string query = $@"Select tm.*,rm.RoleName ,cm.Category,tm.Duration,tm.Rate,tm.Remarks,tm.DoctorAdviceRequired
                            from TaskMaster tm 
                            left Join [{ehrmsDbName}].dbo.RoleMaster rm on tm.Department=rm.RoleID 
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
