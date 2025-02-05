using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class BrandAwarenessAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BrandAwarenessAPIController> _logger;
    private readonly IMapper _mapper;
    public BrandAwarenessAPIController(IUnitOfWork unitOfWork, ILogger<BrandAwarenessAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> BrandAwarenessList()
    {
        try
        {

            string query = "Select * from BrandAwareness where IsActive=1";
            var res = await _unitOfWork.BrandAwareness.GetTableData<BrandAwarenessDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(BrandAwarenessList)}");
            throw;
        }
    }
}
