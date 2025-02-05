using AutoMapper;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class ChannelCodeAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChannelCodeAPIController> _logger;
    private readonly IMapper _mapper;
    public ChannelCodeAPIController(IUnitOfWork unitOfWork, ILogger<ChannelCodeAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> ChannelCodeList()
    {
        try
        {

            string query = "Select * from ChannelCode";
            var res = await _unitOfWork.ChannelCode.GetTableData<ChannelCodeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChannelCodeList)}");
            throw;
        }
    }
}
