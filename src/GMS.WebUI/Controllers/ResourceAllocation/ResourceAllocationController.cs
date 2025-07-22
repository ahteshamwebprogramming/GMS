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
}
