using AutoMapper;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class LeadSourceAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeadSourceAPIController> _logger;
    private readonly IMapper _mapper;
    public LeadSourceAPIController(IUnitOfWork unitOfWork, ILogger<LeadSourceAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> LeadSourceList()
    {
        try
        {

            string query = "Select * from LeadSource where isactive=1";
            var res = await _unitOfWork.LeadSource.GetTableData<LeadSourceDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(LeadSourceList)}");
            throw;
        }
    }
}
