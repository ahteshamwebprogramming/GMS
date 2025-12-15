using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.ResourceAllocation;
using GMS.WebUI.Controllers.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.ResourceAllocation;

public class ResourceAllocationController : Controller
{
    private readonly ILogger<ResourceAllocationController> _logger;
    private readonly CategoryMasterAPIController _categoryMasterAPIController;
    private readonly RoleMasterAPIController _roleMasterAPIController;
    private readonly ResourceAllocationAPIController _resourceAllocationAPIController;
    private readonly GuestsAPIController _guestsAPIController;
    public ResourceAllocationController(ILogger<ResourceAllocationController> logger, CategoryMasterAPIController categoryMasterAPIController, RoleMasterAPIController roleMasterAPIController, ResourceAllocationAPIController resourceAllocationAPIController, GuestsAPIController guestsAPIController)
    {
        _logger = logger;
        _categoryMasterAPIController = categoryMasterAPIController;
        _roleMasterAPIController = roleMasterAPIController;
        _resourceAllocationAPIController = resourceAllocationAPIController;
        _guestsAPIController = guestsAPIController;
    }
    public IActionResult List()
    {
        return View();
    }
    public async Task<IActionResult> ListPartialView()
    {
        ResourceAllocationViewModel dto = new ResourceAllocationViewModel();

        var resourceAllocationRes = await _resourceAllocationAPIController.GetAllSchedules();
        if (resourceAllocationRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resourceAllocationRes).StatusCode == 200)
        {
            dto.ScheduleWithAttributeList = (List<GuestScheduleWithAttributes>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resourceAllocationRes).Value;
        }
        var taskRes = await _guestsAPIController.GetTasks();
        if (taskRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)taskRes).StatusCode == 200)
        {
            dto.Tasks = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)taskRes).Value;
        }
        return PartialView("_list/_roomAllocation", dto);
    }

    public async Task<IActionResult> GetScheduleById(int Id)
    {
        try
        {
            var scheduleRes = await _resourceAllocationAPIController.GetScheduleById(Id);
            if (scheduleRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)scheduleRes).StatusCode == 200)
            {
                var schedule = (GMS.Infrastructure.ViewModels.ResourceAllocation.GuestScheduleWithAttributes?)((Microsoft.AspNetCore.Mvc.ObjectResult)scheduleRes).Value;
                return Ok(schedule);
            }
            return BadRequest("Schedule not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting schedule {nameof(GetScheduleById)}");
            return BadRequest("Error retrieving schedule");
        }
    }

    public async Task<IActionResult> UpdateSchedule([FromBody] GMS.Infrastructure.Models.Guests.GuestScheduleDTO inputDTO)
    {
        try
        {
            if (inputDTO == null || inputDTO.Id <= 0)
            {
                return BadRequest("Invalid schedule data");
            }

            var res = await _guestsAPIController.CreateGuestScheduleByCalendar(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating schedule {nameof(UpdateSchedule)}");
            return BadRequest("Error updating schedule");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSchedule([FromBody] GMS.Infrastructure.Models.Guests.GuestScheduleDTO inputDTO)
    {
        try
        {
            if (inputDTO == null || inputDTO.Id <= 0)
            {
                return BadRequest("Invalid schedule ID");
            }

            var res = await _guestsAPIController.DeleteGuestSchedule(inputDTO.Id);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting schedule {nameof(DeleteSchedule)}");
            return BadRequest("Error deleting schedule");
        }
    }

    public async Task<IActionResult> GetTaskName()
    {
        var res = await _guestsAPIController.GetTasks();
        return res;
    }

    public async Task<IActionResult> GetEmployeeByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetEmployeeByTaskId(inputDTO.Id);
        return res;
    }

    public async Task<IActionResult> GetResourcesByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetResourcesByTaskId(inputDTO.Id);
        return res;
    }

    public async Task<IActionResult> GetTaskByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetTaskByTaskId(inputDTO.Id);
        return res;
    }
}
