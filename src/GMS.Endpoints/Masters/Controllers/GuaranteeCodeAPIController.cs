using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class GuaranteeCodeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GuaranteeCodeAPIController> _logger;
    private readonly IMapper _mapper;
    public GuaranteeCodeAPIController(IUnitOfWork unitOfWork, ILogger<GuaranteeCodeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from GuaranteeCode where IsActive=1";
            var res = await _unitOfWork.GuaranteeCode.GetTableData<GuaranteeCodeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }

    public async Task<IActionResult> GuaranteeCodeById(int Id)
    {
        try
        {
            string query = "Select * from GuaranteeCode where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.GuaranteeCode.GetEntityData<GuaranteeCodeDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GuaranteeCodeById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteGuaranteeCode(int Id)
    {
        try
        {
            string query = "Select * from GuaranteeCode where Id=@Id";
            var param = new { @Id = Id };
            GuaranteeCode? dto = await _unitOfWork.GuaranteeCode.GetEntityData<GuaranteeCode>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.GuaranteeCode.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteGuaranteeCode)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(GuaranteeCodeDTO dto)
    {
        try
        {
            string eQuery = "Select * from GuaranteeCode where IsActive=@IsActive and Code=@Code";
            var eParam = new { @IsAcive = 1, @Code = dto.Code };
            var exists = await _unitOfWork.GuaranteeCode.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Code already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.GuaranteeCode.AddAsync(_mapper.Map<GuaranteeCode>(dto));
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

    public async Task<IActionResult> Update(GuaranteeCodeDTO dto)
    {
        try
        {
            string eQuery = "Select * from GuaranteeCode where IsActive=@IsActive and Code=@Code and Id!=@Id";
            var eParam = new { @IsAcive = 1, @Id = dto.Id, @Code = dto.Code };

            var exists = await _unitOfWork.GuaranteeCode.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This code already exists");
            }
            else
            {
                string query = "Select * from GuaranteeCode where Id=@Id";
                var param = new { @Id = dto.Id };
                GuaranteeCode? guaranteeCode = await _unitOfWork.GuaranteeCode.GetEntityData<GuaranteeCode>(query, param);
                if (guaranteeCode != null)
                {
                    guaranteeCode.Code = dto.Code;

                    var updated = await _unitOfWork.GuaranteeCode.UpdateAsync(guaranteeCode);
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
