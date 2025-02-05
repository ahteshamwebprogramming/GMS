using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class MasterScheduleAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MasterScheduleAPIController> _logger;
    private readonly IMapper _mapper;
    public MasterScheduleAPIController(IUnitOfWork unitOfWork, ILogger<MasterScheduleAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> MasterScheduleList()
    {
        try
        {

            string query1 = "Select * from masterschedule where IsActive=1";
            string query = "Select ms.*,tm.taskname from masterschedule ms Join TaskMaster tm on ms.TaskId=tm.Id where ms.IsActive=1";
            var res = await _unitOfWork.MasterSchedule.GetTableData<MasterScheduleWithChild>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(MasterScheduleList)}");
            throw;
        }
    }
    public async Task<IActionResult> MasterScheduleById(int Id)
    {
        try
        {
            string query = "Select * from masterschedule where IsActive=1 and Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.MasterSchedule.GetEntityData<MasterScheduleDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(MasterScheduleList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteScheduleMaster(int Id)
    {
        try
        {
            string query = "Select * from masterschedule where IsActive=1 and Id=@Id";
            var param = new { @Id = Id };
            MasterSchedule? masterSchedule = await _unitOfWork.MasterSchedule.GetEntityData<MasterSchedule>(query, param);
            if (masterSchedule != null)
            {
                masterSchedule.IsActive = false;
                var updated = await _unitOfWork.MasterSchedule.UpdateAsync(masterSchedule);
                if (updated)
                {
                    return Ok(masterSchedule);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(MasterScheduleList)}");
            throw;
        }
    }
    public async Task<IActionResult> AddMasterSchedule(MasterScheduleDTO dto)
    {
        try
        {
            TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and IsActive=1";
            var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime };

            var exists = await _unitOfWork.MasterSchedule.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.MasterSchedule.AddAsync(_mapper.Map<MasterSchedule>(dto));
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
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(MasterScheduleList)}");
            throw;
        }
    }

    public async Task<IActionResult> UpdateMasterSchedule(MasterScheduleDTO dto)
    {
        try
        {
            TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and Id!=@Id and isactive=1";
            var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime, @Id = dto.Id };

            var exists = await _unitOfWork.MasterSchedule.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from masterschedule where IsActive=1 and Id=@Id";
                var param = new { @Id = dto.Id };
                MasterSchedule? masterSchedule = await _unitOfWork.MasterSchedule.GetEntityData<MasterSchedule>(query, param);
                if (masterSchedule != null)
                {
                    masterSchedule.TaskId = dto.TaskId;
                    masterSchedule.Duration = dto.Duration;
                    masterSchedule.StartTime = dto.StartTime;
                    masterSchedule.EndTime = dto.EndTime;
                    masterSchedule.ModifiedBy = dto.ModifiedBy;
                    masterSchedule.ModifiedDate = dto.ModifiedDate;
                    var updated = await _unitOfWork.MasterSchedule.UpdateAsync(masterSchedule);
                    if (updated)
                    {
                        return Ok(masterSchedule);
                    }
                }
                return BadRequest("Unable to update right now");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(MasterScheduleList)}");
            throw;
        }
    }
}
