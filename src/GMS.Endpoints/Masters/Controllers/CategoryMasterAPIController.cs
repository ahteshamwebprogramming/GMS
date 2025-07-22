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
public class CategoryMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public CategoryMasterAPIController(IUnitOfWork unitOfWork, ILogger<CategoryMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        try
        {

            string query = "Select * from CategoryMaster where IsDeleted=0";
            var res = await _unitOfWork.CategoryMaster.GetTableData<CategoryMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    public async Task<IActionResult> ListWithAttributes()
    {
        try
        {

            string query = @"Select cm.*,rm.RoleName Department from CategoryMaster cm
                            Left Join EHRMS.dbo.RoleMaster rm on cm.DepartmentId=rm.RoleID
                            where IsDeleted=0";
            var res = await _unitOfWork.CategoryMaster.GetTableData<CategoryMasterWithAttributes>(query);
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
    public async Task<IActionResult> CategoryMasterById(int Id)
    {
        try
        {
            string query = "Select * from CategoryMaster where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.CategoryMaster.GetEntityData<CategoryMasterDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CategoryMasterById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteCategoryMaster(int Id)
    {
        try
        {
            string query = "Select * from CategoryMaster where Id=@Id";
            var param = new { @Id = Id };
            CategoryMaster? dto = await _unitOfWork.CategoryMaster.GetEntityData<CategoryMaster>(query, param);
            if (dto != null)
            {
                dto.IsDeleted = true;
                var updated = await _unitOfWork.CategoryMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteCategoryMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> GetCategoriesByDepartment(CategoryMasterDTO inputDTO)
    {
        try
        {
            string query = "Select * from CategoryMaster where IsActive=1 and IsDeleted=0 and DepartmentId=@DepartmentId";
            var param = new { @DepartmentId = inputDTO.DepartmentId };
            var res = await _unitOfWork.CategoryMaster.GetTableData<CategoryMasterDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteCategoryMaster)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(CategoryMasterDTO dto)
    {
        try
        {
            string eQuery = "Select * from CategoryMaster where IsDeleted=0 and Category=@Category";
            var eParam = new { @Category = 1 };
            var exists = await _unitOfWork.CategoryMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Room Type already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.CategoryMaster.AddAsync(_mapper.Map<CategoryMaster>(dto));
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

    public async Task<IActionResult> Update(CategoryMasterDTO dto)
    {
        try
        {
            string eQuery = "Select * from CategoryMaster where IsDeleted=0 and Category=@Category and Id!=@Id";
            var eParam = new { @Category = dto.Category, @Id = dto.Id };

            var exists = await _unitOfWork.CategoryMaster.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from CategoryMaster where Id=@Id";
                var param = new { @Id = dto.Id };
                CategoryMaster? roomType = await _unitOfWork.CategoryMaster.GetEntityData<CategoryMaster>(query, param);
                if (roomType != null)
                {
                    roomType.Category = dto.Category;
                    roomType.DepartmentId = dto.DepartmentId;
                    roomType.ModifiedBy = dto.ModifiedBy;
                    roomType.ModifiedDate = dto.ModifiedDate;

                    var updated = await _unitOfWork.CategoryMaster.UpdateAsync(roomType);
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
    public async Task<IActionResult> ManageCategoryMasterStatus(CategoryMasterDTO inputDto)
    {
        try
        {
            string query = "Select * from CategoryMaster where Id=@Id";
            var param = new { @Id = inputDto.Id };
            CategoryMaster? dto = await _unitOfWork.CategoryMaster.GetEntityData<CategoryMaster>(query, param);
            if (dto != null)
            {
                dto.IsActive = inputDto.IsActive;
                var updated = await _unitOfWork.CategoryMaster.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteCategoryMaster)}");
            throw;
        }
    }
}
