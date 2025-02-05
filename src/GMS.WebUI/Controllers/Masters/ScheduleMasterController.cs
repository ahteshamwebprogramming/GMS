using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Masters;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class ScheduleMasterController : Controller
{
    private readonly ILogger<ScheduleMasterController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly MasterScheduleAPIController _masterScheduleAPIController;
    private readonly TaskMasterAPIController _taskMasterAPIController;

    public ScheduleMasterController(ILogger<ScheduleMasterController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, MasterScheduleAPIController masterScheduleAPIController, TaskMasterAPIController taskMasterAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _masterScheduleAPIController = masterScheduleAPIController;
        _taskMasterAPIController = taskMasterAPIController;
    }
    public async Task<IActionResult> ScheduleMasterList()
    {
        MasterScheduleViewModel dto = new MasterScheduleViewModel();

        var res = await _masterScheduleAPIController.MasterScheduleList();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.MasterScheduleWithChildren = (List<MasterScheduleWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> SaveMasterSchedule(MasterScheduleDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                dataVM.CreatedDate = DateTime.Now;
                dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _masterScheduleAPIController.AddMasterSchedule(dataVM);
                return res;
            }
            else
            {
                dataVM.ModifiedDate = DateTime.Now;
                dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _masterScheduleAPIController.UpdateMasterSchedule(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> MasterScheduleListPartialView()
    {
        MasterScheduleViewModel dto = new MasterScheduleViewModel();

        var res = await _masterScheduleAPIController.MasterScheduleList();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.MasterScheduleWithChildren = (List<MasterScheduleWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return PartialView("_scheduleMasterList/_list", dto);
    }
    public async Task<IActionResult> AddMasterSchedulePartialView([FromBody] MasterScheduleDTO inputDTO)
    {
        MasterScheduleViewModel viewModel = new MasterScheduleViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _masterScheduleAPIController.MasterScheduleById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.MasterSchedule = (MasterScheduleDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        var resTask = await _taskMasterAPIController.List();
        if (resTask != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resTask).StatusCode == 200)
        {
            viewModel.TaskList = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resTask).Value;
        }

        return PartialView("_scheduleMasterList/_add", viewModel);
    }
    public async Task<IActionResult> DeleteScheduleMaster([FromBody] MasterScheduleDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _masterScheduleAPIController.DeleteScheduleMaster(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
