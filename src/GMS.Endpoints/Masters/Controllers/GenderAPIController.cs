using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class GenderAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenderAPIController> _logger;
    private readonly IMapper _mapper;
    public GenderAPIController(IUnitOfWork unitOfWork, ILogger<GenderAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GenderList()
    {
        try
        {
            string query = "Select * from GenderMaster where IsActive=1";
            var res = await _unitOfWork.GenderMaster.GetTableData<GenderMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GenderList)}");
            throw;
        }
    }
}
