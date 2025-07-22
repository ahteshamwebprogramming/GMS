using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.RoleMenuMapping;
using GMS.Infrastructure.ViewModels.EHRMSLogin;
using GMS.Infrastructure.ViewModels.RoleMenuMapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GMS.Endpoints.Accounts;

[Route("api/[controller]")]
[ApiController]
public class WorkerMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkerMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public WorkerMasterAPIController(IUnitOfWork unitOfWork, ILogger<WorkerMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    [HttpPost]
    public async Task<IActionResult> GetEmployeeDetails()
    {
        try
        {
            string query = "Select WorkerID,EMPID,FirstName,MiddleName,LastName,WorkerName from WorkerMaster";
            var res = await _unitOfWork.EHRMSLogin.GetTableData<WorkerMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetEmployeeDetails)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetMenuListForMapping(int RoleId)
    {
        try
        {
            string query = @"SELECT 
                            ml.*,
                            CASE 
                                WHEN rmm.MenuId IS NOT NULL THEN 1 
                                ELSE 0 
                            END AS Selected
                        FROM MenuList ml
                        LEFT JOIN RoleMenuMapping rmm ON ml.Id = rmm.MenuId AND rmm.RoleId = @RoleId where DefaultMenu=0 order by SNo";
            var res = await _unitOfWork.GenOperations.GetTableData<MenuListWithAttr>(query, new { @RoleId = RoleId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetEmployeeDetails)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRolesForMapping()
    {
        try
        {
            string query = @"Select * from RoleMaster where isActive='Y'";
            var res = await _unitOfWork.RoleMaster.GetTableData<RoleMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetRolesForMapping)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveMapping(RoleMenuMappingPostModel model)
    {
        try
        {
            // Example: Remove existing mappings for this role
            await _unitOfWork.GenOperations.RunSQLCommand("DELETE FROM RoleMenuMapping WHERE RoleId = @RoleId", new { model.RoleId });
            //_dapper.Execute("DELETE FROM RoleMenuMapping WHERE RoleId = @RoleId", new { model.RoleId });

            // Insert new mappings
            foreach (var menuId in model.MenuIds)
            {
                var parameters = new
                {
                    RoleId = model.RoleId,
                    DesignationId = 0, // optional: update if used
                    DepartmentId = 0,  // optional: update if used
                    MenuId = menuId,
                    IsActive = true,
                    CreatedDate = DateTime.Now,                    
                };

                await _unitOfWork.GenOperations.RunSQLCommand(@"
                    INSERT INTO RoleMenuMapping (RoleId, DesignationId, DepartmentId, MenuId, IsActive, CreatedDate)
                    VALUES (@RoleId, @DesignationId, @DepartmentId, @MenuId, @IsActive, @CreatedDate)", parameters);

                //    _dapper.Execute(@"
                //INSERT INTO RoleMenuMapping (RoleId, DesignationId, DepartmentId, MenuId, IsActive, CreatedDate, CreatedBy)
                //VALUES (@RoleId, @DesignationId, @DepartmentId, @MenuId, @IsActive, @CreatedDate, @CreatedBy)", parameters);
            }
            return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetRolesForMapping)}");
            throw;
        }
    }
}
