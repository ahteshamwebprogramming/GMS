using AutoMapper;
using GMS.Core.Repository;
using GMS.Core.Services;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.RoleMenuMapping;
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
    private readonly IUserPermissionService? _userPermissionService;
    
    public AccountsAPIController(IUnitOfWork unitOfWork, ILogger<AccountsAPIController> logger, IMapper mapper, IUserPermissionService? userPermissionService = null)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _userPermissionService = userPermissionService;
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
            string query = "Select el.*,wm.WorkerName,wm.RoleId,rm.RoleName from EHRMSLogin el Join WorkerMaster wm on el.WorkerID=wm.WorkerID Left Join RoleMaster rm on wm.RoleID=rm.RoleID where el.WorkerCode=@WorkerCode and el.UserPassword=@UserPassword";
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
    [HttpPost]
    public async Task<List<MenuListDTO>> GetMenuDetails(int RoleId, int? UserId = null)
    {
        try
        {
            // If UserPermissionService is available and UserId is provided, use effective permissions
            if (_userPermissionService != null && UserId.HasValue && UserId.Value > 0)
            {
                var effectivePermissions = await _userPermissionService.GetEffectivePermissionsAsync(UserId.Value);
                var allowedPageIds = effectivePermissions
                    .Where(p => (p.UserOverride == "Allow") || (p.UserOverride == null && p.RoleCanView))
                    .Select(p => p.PageId)
                    .ToHashSet();
                
                if (allowedPageIds.Any())
                {
                    string query = @"
                        SELECT ml.* 
                        FROM MenuList ml 
                        WHERE ml.Id IN @PageIds 
                            AND ml.DefaultMenu = 0 
                            AND ml.IsActive = 1
                        ORDER BY ml.SNo";
                    var parameters = new { PageIds = allowedPageIds.ToList() };
                    var res = await _unitOfWork.MenuList.GetTableData<MenuListDTO>(query, parameters);
                    return res ?? new List<MenuListDTO>();
                }
                else
                {
                    return new List<MenuListDTO>();
                }
            }
            
            // Fallback to role-based permissions (original behavior)
            string roleQuery = "Select ml.* from RoleMenuMapping rmm Left Join MenuList ml on rmm.MenuId = ml.Id where RoleId=@RoleId and DefaultMenu=0 and ml.IsActive=1 order by SNo";
            var roleParameters = new { @RoleId = RoleId };
            var roleRes = await _unitOfWork.MenuList.GetTableData<MenuListDTO>(roleQuery, roleParameters);
            return roleRes ?? new List<MenuListDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Menu Details {nameof(GetMenuDetails)}");
            throw;
        }
    }
}
