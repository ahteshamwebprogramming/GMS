using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class TaskMasterController : Controller
{
    private readonly ILogger<ScheduleMasterController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly TaskMasterAPIController _taskMasterAPIController;
    private readonly RoleMasterAPIController _roleMasterAPIController;

    public TaskMasterController(ILogger<ScheduleMasterController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, TaskMasterAPIController taskMasterAPIController, RoleMasterAPIController roleMasterAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _taskMasterAPIController = taskMasterAPIController;
        _roleMasterAPIController = roleMasterAPIController;
    }
    public async Task<IActionResult> TaskMasterList()
    {
        TaskMasterViewModel dto = new TaskMasterViewModel();

        var res = await _taskMasterAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.TaskMasters = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> SaveTaskMaster(TaskMasterDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                dataVM.IsDeleted = false;
                //dataVM.CreatedDate = DateTime.Now;
                //dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _taskMasterAPIController.Add(dataVM);
                return res;
            }
            else
            {
                //dataVM.ModifiedDate = DateTime.Now;
                //dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _taskMasterAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> TaskMasterListPartialView()
    {
        TaskMasterViewModel dto = new TaskMasterViewModel();

        var res = await _taskMasterAPIController.ListWithChild();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.TaskMasterWithChildren = (List<TaskMasterWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return PartialView("_taskMasterList/_list", dto);
    }
    public async Task<IActionResult> AddTaskMasterPartialView([FromBody] TaskMasterDTO inputDTO)
    {
        TaskMasterViewModel viewModel = new TaskMasterViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _taskMasterAPIController.TaskMasterById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.TaskMaster = (TaskMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }

        var resRoles = await _roleMasterAPIController.List();
        if (resRoles != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).StatusCode == 200)
        {
            viewModel.Roles = (List<RoleMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).Value;
        }


        return PartialView("_taskMasterList/_add", viewModel);
    }
    public async Task<IActionResult> DeleteTaskMaster([FromBody] TaskMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _taskMasterAPIController.DeleteTaskMaster(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
    public async Task<IActionResult> ManageTaskMasterStatus([FromBody] TaskMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _taskMasterAPIController.ManageTaskMasterStatus(inputDTO);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
