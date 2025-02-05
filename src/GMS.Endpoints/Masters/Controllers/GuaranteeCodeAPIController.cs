using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class GuaranteeCodeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GuaranteeCodeAPIController> _logger;
    private readonly IMapper _mapper;
    public GuaranteeCodeAPIController(IUnitOfWork unitOfWork, ILogger<GuaranteeCodeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GuaranteeCodeList()
    {
        try
        {

            string query = "Select * from GuaranteeCode";
            var res = await _unitOfWork.GuaranteeCode.GetTableData<GuaranteeCodeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GuaranteeCodeList)}");
            throw;
        }
    }
}
