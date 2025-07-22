using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.RoleMenuMapping;
using GMS.Infrastructure.ViewModels.EHRMSLogin;
using GMS.Infrastructure.ViewModels.RoleMenuMapping;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GMS.WebUI.Controllers.UserManagement
{
    public class UserManagementController : Controller
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly GuestsAPIController _guestsAPIController;
        private readonly AccountsAPIController _accountsAPIController;
        private readonly WorkerMasterAPIController _workerMasterAPIController;
        public UserManagementController(ILogger<UserManagementController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, WorkerMasterAPIController workerMasterAPIController)
        {
            _logger = logger;
            _guestsAPIController = guestsAPIController;
            _accountsAPIController = accountsAPIController;
            _workerMasterAPIController = workerMasterAPIController;
        }
        public async Task<IActionResult> UserList()
        {
            WorkerMasterViewModel dto = new WorkerMasterViewModel();
            var res = await _workerMasterAPIController.GetEmployeeDetails();
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.WorkerMasterList = (List<WorkerMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> RoleMenuMapping()
        {
            RoleMenuMappingViewModel dto = new RoleMenuMappingViewModel();

            var resRoles = await _workerMasterAPIController.GetRolesForMapping();
            if (resRoles != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).StatusCode == 200)
            {
                dto.Roles = (List<RoleMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).Value;
            }


            var res = await _workerMasterAPIController.GetMenuListForMapping(0);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.MenuListWithAttrs = (List<MenuListWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> RoleMenuMappingPartial([FromBody] RoleMenuMappingDTO inputDTO)
        {
            RoleMenuMappingViewModel dto = new RoleMenuMappingViewModel();

            var res = await _workerMasterAPIController.GetMenuListForMapping(inputDTO.RoleId ?? 0);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.MenuListWithAttrs = (List<MenuListWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return PartialView("_roleMenuMapping/_roleMenuMapping", dto);
        }
        [HttpPost]
        public async Task<IActionResult> SaveMapping([FromBody] RoleMenuMappingPostModel model)
        {
            if (model == null || model.RoleId == 0 || model.MenuIds == null || !model.MenuIds.Any())
            {
                return BadRequest("Invalid data");
            }

            await _workerMasterAPIController.SaveMapping(model);

            //// Example: Remove existing mappings for this role
            //_dapper.Execute("DELETE FROM RoleMenuMapping WHERE RoleId = @RoleId", new { model.RoleId });

            //// Insert new mappings
            //foreach (var menuId in model.MenuIds)
            //{
            //    var parameters = new
            //    {
            //        RoleId = model.RoleId,
            //        DesignationId = 0, // optional: update if used
            //        DepartmentId = 0,  // optional: update if used
            //        MenuId = menuId,
            //        IsActive = true,
            //        CreatedDate = DateTime.Now,
            //        CreatedBy = User.Identity.Name // or your logic
            //    };

            //    _dapper.Execute(@"
            //INSERT INTO RoleMenuMapping (RoleId, DesignationId, DepartmentId, MenuId, IsActive, CreatedDate, CreatedBy)
            //VALUES (@RoleId, @DesignationId, @DepartmentId, @MenuId, @IsActive, @CreatedDate, @CreatedBy)", parameters);
            //}

            return Ok(new { success = true });
        }

    }
}
