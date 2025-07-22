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
public class ChannelCodeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChannelCodeAPIController> _logger;
    private readonly IMapper _mapper;
    public ChannelCodeAPIController(IUnitOfWork unitOfWork, ILogger<ChannelCodeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from ChannelCode where IsActive=1";
            var res = await _unitOfWork.ChannelCode.GetTableData<ChannelCodeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }

    public async Task<IActionResult> ChannelCodeById(int Id)
    {
        try
        {
            string query = "Select * from ChannelCode where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.ChannelCode.GetEntityData<ChannelCodeDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChannelCodeById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteChannelCode(int Id)
    {
        try
        {
            string query = "Select * from ChannelCode where Id=@Id";
            var param = new { @Id = Id };
            ChannelCode? dto = await _unitOfWork.ChannelCode.GetEntityData<ChannelCode>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.ChannelCode.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteChannelCode)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(ChannelCodeDTO dto)
    {
        try
        {
            string eQuery = "Select * from ChannelCode where IsActive=@IsActive and Code=@Code";
            var eParam = new { @IsAcive = 1, @Code = dto.Code };
            var exists = await _unitOfWork.ChannelCode.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Code already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.ChannelCode.AddAsync(_mapper.Map<ChannelCode>(dto));
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

    public async Task<IActionResult> Update(ChannelCodeDTO dto)
    {
        try
        {
            string eQuery = "Select * from ChannelCode where IsActive=@IsActive and Code=@Code and Id!=@Id";
            var eParam = new { @IsAcive = 1, @Id = dto.Id, @Code = dto.Code };

            var exists = await _unitOfWork.ChannelCode.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This code already exists");
            }
            else
            {
                string query = "Select * from ChannelCode where Id=@Id";
                var param = new { @Id = dto.Id };
                ChannelCode? guaranteeCode = await _unitOfWork.ChannelCode.GetEntityData<ChannelCode>(query, param);
                if (guaranteeCode != null)
                {
                    guaranteeCode.Code = dto.Code;

                    var updated = await _unitOfWork.ChannelCode.UpdateAsync(guaranteeCode);
                    if (updated)
                    {
                        return Ok(guaranteeCode);
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
