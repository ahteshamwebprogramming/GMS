using AutoMapper;
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
    public async Task<IActionResult> ServicesList()
    {
        try
        {
            string query = "Select * from Services where Status=1";
            var res = await _unitOfWork.GenderMaster.GetTableData<ServicesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ServicesList)}");
            throw;
        }
    }
}
