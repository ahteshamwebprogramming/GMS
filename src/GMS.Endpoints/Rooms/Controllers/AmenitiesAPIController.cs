using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class AmenitiesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AmenitiesAPIController> _logger;
    private readonly IMapper _mapper;
    public AmenitiesAPIController(IUnitOfWork unitOfWork, ILogger<AmenitiesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        try
        {

            string query = @"Select a.*,ac.AmenetiesCategoryName AmenityCategory from Amenities a
                            Left join AmenetiesCategory ac on a.AmenityCategoryId=ac.Id
                            where a.IsActive=1";
            var res = await _unitOfWork.Amenities.GetTableData<AmenitiesDTO>(query);
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
    public async Task<IActionResult> AmenitiesById(int Id)
    {
        try
        {
            string query = "Select * from Amenities where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.Amenities.GetEntityData<AmenitiesDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AmenitiesById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteAmenities(int Id)
    {
        try
        {
            string query = "Select * from Amenities where Id=@Id";
            var param = new { @Id = Id };
            Amenities? dto = await _unitOfWork.Amenities.GetEntityData<Amenities>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.Amenities.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteAmenities)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(AmenitiesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Amenities where IsActive=@IsActive and AmenityName=@AmenityName";
            var eParam = new { @IsActive = 1, @AmenityName = dto.AmenityName };
            var exists = await _unitOfWork.Amenities.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Category already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.Amenities.AddAsync(_mapper.Map<Amenities>(dto));
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

    public async Task<IActionResult> Update(AmenitiesDTO dto)
    {
        try
        {
            string eQuery = "Select * from Amenities where IsActive=@IsActive and AmenityName=@AmenityName and Id!=@Id";
            var eParam = new { @IsActive = 1, @Id = dto.Id, @AmenityName = dto.AmenityName };

            var exists = await _unitOfWork.Amenities.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This range already exists");
            }
            else
            {
                string query = "Select * from Amenities where Id=@Id";
                var param = new { @Id = dto.Id };
                Amenities? amenetiesCategory = await _unitOfWork.Amenities.GetEntityData<Amenities>(query, param);
                if (amenetiesCategory != null)
                {
                    amenetiesCategory.AmenityName = dto.AmenityName;
                    amenetiesCategory.AmenityCategoryId = dto.AmenityCategoryId;
                    

                    var updated = await _unitOfWork.Amenities.UpdateAsync(amenetiesCategory);
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
