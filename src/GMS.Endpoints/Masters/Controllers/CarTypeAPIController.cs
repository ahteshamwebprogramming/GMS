using AutoMapper;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class CarTypeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CarTypeAPIController> _logger;
    private readonly IMapper _mapper;
    public CarTypeAPIController(IUnitOfWork unitOfWork, ILogger<CarTypeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> CarTypeList()
    {
        try
        {
            
            string query = "Select * from CarType";
            var res = await _unitOfWork.GenderMaster.GetTableData<CarTypeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CarTypeList)}");
            throw;
        }
    }
}
