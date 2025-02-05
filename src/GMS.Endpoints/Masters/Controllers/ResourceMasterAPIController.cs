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
public class ResourceMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResourceMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public ResourceMasterAPIController(IUnitOfWork unitOfWork, ILogger<ResourceMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {

            string query = "Select * from ResourceMaster where IsActive=1 and IsDeleted=0";
            var res = await _unitOfWork.ResourceMaster.GetTableData<ResourceMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    public async Task<IActionResult> ListWithChild()
    {
        try
        {

            string query = "Select tm.*,rm.RoleName from ResourceMaster tm Join EHRMS.dbo.RoleMaster rm on tm.DepartmentId=rm.RoleID where tm.IsDeleted=0";
            var res = await _unitOfWork.ResourceMaster.GetTableData<ResourceMasterWithChild>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    public async Task<IActionResult> ResourceMasterById(int Id)
    {
        try
        {
            string query = "Select * from ResourceMaster where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.ResourceMaster.GetEntityData<ResourceMasterDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ResourceMasterById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteResourceMaster(int Id)
    {
        try
        {
            string query = "Select * from ResourceMaster where Id=@Id";
            var param = new { @Id = Id };
            ResourceMaster? dto = await _unitOfWork.ResourceMaster.GetEntityData<ResourceMaster>(query, param);
            if (dto != null)
            {
                dto.IsDeleted = true;
                var updated = await _unitOfWork.ResourceMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteResourceMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> ManageResourceMasterStatus(ResourceMasterDTO inputDto)
    {
        try
        {
            string query = "Select * from ResourceMaster where Id=@Id";
            var param = new { @Id = inputDto.Id };
            ResourceMaster? dto = await _unitOfWork.ResourceMaster.GetEntityData<ResourceMaster>(query, param);
            if (dto != null)
            {
                dto.IsActive = inputDto.IsActive;
                var updated = await _unitOfWork.ResourceMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteResourceMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> Add(ResourceMasterDTO dto)
    {
        try
        {
            //TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            //TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            //dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            //string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and IsActive=1";

            string eQuery = "Select * from ResourceMaster where DepartmentId=@DepartmentId and ResourceName=@ResourceName and IsDeleted=0";
            //var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime };
            var eParam = new { @DepartmentId = dto.DepartmentId, @ResourceName = dto.ResourceName };


            var exists = await _unitOfWork.ResourceMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.ResourceMaster.AddAsync(_mapper.Map<ResourceMaster>(dto));
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

    public async Task<IActionResult> Update(ResourceMasterDTO dto)
    {
        try
        {
            //TimeSpan durationSpan = dto.Duration.ToTimeSpan();
            //TimeSpan adjustedDurationSpan = durationSpan - TimeSpan.FromSeconds(1);
            //dto.EndTime = dto.StartTime.Add(adjustedDurationSpan);

            //string eQuery = "Select * from MasterSchedule ms where \r\n((@starttime BETWEEN ms.StartTime AND ms.EndTime) OR \r\n(@endtime BETWEEN ms.StartTime AND ms.EndTime) or \r\n(@starttime<=ms.StartTime and @endtime > ms.EndTime)) and Id!=@Id and isactive=1";
            string eQuery = "Select * from ResourceMaster where DepartmentId=@DepartmentId and ResourceName=@ResourceName and IsDeleted=0 and Id!=@Id";
            //var eParam = new { starttime = dto.StartTime, endtime = dto.EndTime, @Id = dto.Id };
            var eParam = new { @DepartmentId = dto.DepartmentId, @ResourceName = dto.ResourceName, @Id = dto.Id };

            var exists = await _unitOfWork.ResourceMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from ResourceMaster where IsActive=1 and Id=@Id";
                var param = new { @Id = dto.Id };
                ResourceMaster? resourceMaster = await _unitOfWork.ResourceMaster.GetEntityData<ResourceMaster>(query, param);
                if (resourceMaster != null)
                {
                    resourceMaster.ResourceName = dto.ResourceName;
                    resourceMaster.DepartmentId = dto.DepartmentId;

                    var updated = await _unitOfWork.ResourceMaster.UpdateAsync(resourceMaster);
                    if (updated)
                    {
                        return Ok(resourceMaster);
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
