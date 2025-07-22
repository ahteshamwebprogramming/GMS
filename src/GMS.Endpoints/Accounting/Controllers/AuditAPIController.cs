using AutoMapper;
using Dapper;
using GMS.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GMS.Endpoints.Accounting;

[Route("api/[controller]")]
[ApiController]
public class AuditAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditAPIController> _logger;
    private readonly IMapper _mapper;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public AuditAPIController(IUnitOfWork unitOfWork, ILogger<AuditAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
    }
    public async Task<IActionResult> RunAudit(int UserId)
    {
        try
        {
            string sp = "RunNightAudit";
            var param = new DynamicParameters();
            param.Add("@UserId", UserId);
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync(sp, param);
            //await DbConnection.ExecuteAsync("RunNightAudit", p, commandType: CommandType.StoredProcedure);
            int result = param.Get<int>("@Status");
            if (result > 0)
            {
                return Ok("");
            }
            else
            {
                return BadRequest("");
            }


            //string sp = "RunNightAudit";
            //var param = new { @UserId = UserId };
            //var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync(sp, param);
            //if (res > 0)
            //{
            //    return Ok(res);
            //}
            //else
            //{
            //    return BadRequest(res);
            //}

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RunAudit)}");
            throw;
        }
    }
}
