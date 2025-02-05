using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.ViewModels.EHRMSLogin;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GMS.Endpoints.Accounts;

[Route("api/[controller]")]
[ApiController]
public class AccountsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountsAPIController> _logger;
    private readonly IMapper _mapper;
    public AccountsAPIController(IUnitOfWork unitOfWork, ILogger<AccountsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetLoginList()
    {
        try
        {
            string query = "Select top 100 * from EHRMSLogin";
            var res = await _unitOfWork.EHRMSLogin.GetTableData<EHRMSLoginDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetLoginList)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetLoginDetail(EHRMSLoginDTO inputDTO)
    {
        try
        {
            string query = "Select el.*,wm.WorkerName from EHRMSLogin el Join WorkerMaster wm on el.WorkerID=wm.WorkerID where el.WorkerCode=@WorkerCode and el.UserPassword=@UserPassword";
            var parameters = new { WorkerCode = inputDTO.WorkerCode, UserPassword = inputDTO.UserPassword };
            var res = await _unitOfWork.EHRMSLogin.GetEntityData<EHRMSLoginWithChild>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }
}
