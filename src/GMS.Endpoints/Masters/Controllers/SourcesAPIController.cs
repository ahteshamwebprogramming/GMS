using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class SourcesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SourcesAPIController> _logger;
    private readonly IMapper _mapper;
    public SourcesAPIController(IUnitOfWork unitOfWork, ILogger<SourcesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from Sources where status=1";
            var res = await _unitOfWork.Sources.GetTableData<SourcesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }

    public async Task<IActionResult> SourcesById(int Id)
    {
        try
        {
            string query = "Select * from Sources where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.Sources.GetEntityData<SourcesDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SourcesById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteSources(int Id)
    {
        try
        {
            string query = "Select * from Sources where Id=@Id";
            var param = new { @Id = Id };
            Sources? dto = await _unitOfWork.Sources.GetEntityData<Sources>(query, param);
            if (dto != null)
            {
                dto.Status = 0;
                var updated = await _unitOfWork.Sources.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteSources)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(SourcesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Sources where IsActive=@IsActive and name=@name";
            var eParam = new { @IsAcive = 1, @name = dto.Name };
            var exists = await _unitOfWork.Sources.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Code already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.Sources.AddAsync(_mapper.Map<Sources>(dto));
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

    public async Task<IActionResult> Update(SourcesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Sources where IsActive=@IsActive and Name=@Name and Id!=@Id";
            var eParam = new { @IsAcive = 1, @Id = dto.Id, @Name = dto.Name };

            var exists = await _unitOfWork.Sources.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This code already exists");
            }
            else
            {
                string query = "Select * from Sources where Id=@Id";
                var param = new { @Id = dto.Id };
                Sources? Sources = await _unitOfWork.Sources.GetEntityData<Sources>(query, param);
                if (Sources != null)
                {
                    Sources.Name = dto.Name;

                    var updated = await _unitOfWork.Sources.UpdateAsync(Sources);
                    if (updated)
                    {
                        return Ok(Sources);
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
