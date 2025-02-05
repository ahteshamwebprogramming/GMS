using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class RoomType1APIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomType1APIController> _logger;
    private readonly IMapper _mapper;
    public RoomType1APIController(IUnitOfWork unitOfWork, ILogger<RoomType1APIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> RoomTypeList()
    {
        try
        {
            string query = "Select * from RoomType where Status=1";
            var res = await _unitOfWork.RoomType.GetTableData<RoomTypeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypeList)}");
            throw;
        }
    }
}
