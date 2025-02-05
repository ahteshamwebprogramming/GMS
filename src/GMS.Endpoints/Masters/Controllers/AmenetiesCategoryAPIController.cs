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
public class AmenetiesCategoryAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AmenetiesCategoryAPIController> _logger;
    private readonly IMapper _mapper;
    public AmenetiesCategoryAPIController(IUnitOfWork unitOfWork, ILogger<AmenetiesCategoryAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        try
        {

            string query = "Select * from AmenetiesCategory where IsActive=1";
            var res = await _unitOfWork.AmenetiesCategory.GetTableData<AmenetiesCategoryDTO>(query);
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
    public async Task<IActionResult> AmenetiesCategoryById(int Id)
    {
        try
        {
            string query = "Select * from AmenetiesCategory where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.AmenetiesCategory.GetEntityData<AmenetiesCategoryDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AmenetiesCategoryById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteAmenetiesCategory(int Id)
    {
        try
        {
            string query = "Select * from AmenetiesCategory where Id=@Id";
            var param = new { @Id = Id };
            AmenetiesCategory? dto = await _unitOfWork.AmenetiesCategory.GetEntityData<AmenetiesCategory>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.AmenetiesCategory.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteAmenetiesCategory)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(AmenetiesCategoryDTO dto)
    {
        try
        {
            string eQuery = "Select * from AmenetiesCategory where IsActive=@IsActive and AmenetiesCategoryName=@AmenetiesCategoryName";
            var eParam = new { @IsActive = 1, @AmenetiesCategoryName = dto.AmenetiesCategoryName };
            var exists = await _unitOfWork.AmenetiesCategory.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Category already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.AmenetiesCategory.AddAsync(_mapper.Map<AmenetiesCategory>(dto));
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

    public async Task<IActionResult> Update(AmenetiesCategoryDTO dto)
    {
        try
        {
            string eQuery = "Select * from AmenetiesCategory where IsActive=@IsActive and AmenetiesCategoryName=@AmenetiesCategoryName and Id!=@Id";
            var eParam = new { @IsActive = 1, @Id = dto.Id, @AmenetiesCategoryName = dto.AmenetiesCategoryName };

            var exists = await _unitOfWork.AmenetiesCategory.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from AmenetiesCategory where Id=@Id";
                var param = new { @Id = dto.Id };
                AmenetiesCategory? amenetiesCategory = await _unitOfWork.AmenetiesCategory.GetEntityData<AmenetiesCategory>(query, param);
                if (amenetiesCategory != null)
                {
                    amenetiesCategory.AmenetiesCategoryName = dto.AmenetiesCategoryName;
                    

                    var updated = await _unitOfWork.AmenetiesCategory.UpdateAsync(amenetiesCategory);
                    if (updated)
                    {
                        return Ok(amenetiesCategory);
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
