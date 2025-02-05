using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class MSTCategoriesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MSTCategoriesAPIController> _logger;
    private readonly IMapper _mapper;
    public MSTCategoriesAPIController(IUnitOfWork unitOfWork, ILogger<MSTCategoriesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> CategoriesList(int Id)
    {
        try
        {
            var parameters = new { ServiceID = Id };
            string query = "Select * from MstCategory where ServiceId=@ServiceID and Status=1";
            var res = await _unitOfWork.GenderMaster.GetTableData<MstCategoryDTO>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CategoriesList)}");
            throw;
        }
    }
}
