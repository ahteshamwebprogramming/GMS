using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;
[Route("api/[controller]")]
[ApiController]
public class ServicesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ServicesAPIController> _logger;
    private readonly IMapper _mapper;
    public ServicesAPIController(IUnitOfWork unitOfWork, ILogger<ServicesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from Services where Status=1 order by Service asc";
            var res = await _unitOfWork.GenderMaster.GetTableData<ServicesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    public async Task<IActionResult> ServiceById(int Id)
    {
        try
        {
            string query = "Select * from Services where ID=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.Services.GetEntityData<ServicesDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ServiceById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteServices(int Id)
    {
        try
        {
            string query = "Select * from Services where ID=@Id";
            var param = new { @Id = Id };
            Services? dto = await _unitOfWork.Services.GetEntityData<Services>(query, param);
            if (dto != null)
            {
                dto.Status = 0;
                var updated = await _unitOfWork.Services.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteServices)}");
            throw;
        }
    }
    public async Task<IActionResult> Add(ServicesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Services where [Service]=@Service and Status=1";
            var eParam = new { @Status = 1, @Service = dto.Service };
            var exists = await _unitOfWork.Services.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Package already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.Services.AddAsync(_mapper.Map<Services>(dto));
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
    public async Task<IActionResult> Update(ServicesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Services where Status=@Status and [Service]=@Service and Id!=@Id";
            var eParam = new { @Status = 1, @Id = dto.Id, @Service = dto.Service };

            var exists = await _unitOfWork.Services.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Service already exists");
            }
            else
            {
                string query = "Select * from Services where Id=@Id";
                var param = new { @Id = dto.Id };
                Services? services = await _unitOfWork.Services.GetEntityData<Services>(query, param);
                if (services != null)
                {
                    services.Service = dto.Service;
                    services.MinimumNight = dto.MinimumNight;
                    services.MaximumNight = dto.MaximumNight;
                    services.Price = dto.Price;
                    services.Description = dto.Description;

                    var updated = await _unitOfWork.Services.UpdateAsync(services);
                    if (updated)
                    {
                        return Ok(services);
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
