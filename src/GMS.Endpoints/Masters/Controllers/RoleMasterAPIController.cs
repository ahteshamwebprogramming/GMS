using AutoMapper;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class RoleMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public RoleMasterAPIController(IUnitOfWork unitOfWork, ILogger<RoleMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> List()
    {
        try
        {

            string query = "Select RoleID Id,RoleName [Role] from EHRMS.dbo.Rolemaster where isActive='Y'";
            var res = await _unitOfWork.GenOperations.GetTableData<RoleMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
}
