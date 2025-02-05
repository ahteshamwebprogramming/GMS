using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class ResourceMasterController : Controller
{
    private readonly ILogger<ScheduleMasterController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly ResourceMasterAPIController _resourceMasterAPIController;
    private readonly RoleMasterAPIController _roleMasterAPIController;
    public ResourceMasterController(ILogger<ScheduleMasterController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, ResourceMasterAPIController resourceMasterAPIController, RoleMasterAPIController roleMasterAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _resourceMasterAPIController = resourceMasterAPIController;
        _roleMasterAPIController = roleMasterAPIController;
    }
    public async Task<IActionResult> ResourceMasterList()
    {
        ResourceMasterViewModel dto = new ResourceMasterViewModel();

        var res = await _resourceMasterAPIController.ListWithChild();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.ResourceMasterWithChildren = (List<ResourceMasterWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> SaveResourceMaster(ResourceMasterDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                dataVM.IsDeleted = false;
                //dataVM.CreatedDate = DateTime.Now;
                //dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _resourceMasterAPIController.Add(dataVM);
                return res;
            }
            else
            {
                //dataVM.ModifiedDate = DateTime.Now;
                //dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _resourceMasterAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> ResourceMasterListPartialView()
    {
        ResourceMasterViewModel dto = new ResourceMasterViewModel();

        var res = await _resourceMasterAPIController.ListWithChild();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.ResourceMasterWithChildren = (List<ResourceMasterWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return PartialView("_resourceMasterList/_list", dto);
    }
    public async Task<IActionResult> AddResourceMasterPartialView([FromBody] ResourceMasterDTO inputDTO)
    {
        ResourceMasterViewModel viewModel = new ResourceMasterViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _resourceMasterAPIController.ResourceMasterById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.ResourceMaster = (ResourceMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        var resRoles = await _roleMasterAPIController.List();
        if (resRoles != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).StatusCode == 200)
        {
            viewModel.Roles = (List<RoleMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).Value;
        }
        return PartialView("_resourceMasterList/_add", viewModel);
    }
    public async Task<IActionResult> DeleteResourceMaster([FromBody] ResourceMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _resourceMasterAPIController.DeleteResourceMaster(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }

    public async Task<IActionResult> ManageResourceMasterStatus([FromBody] ResourceMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _resourceMasterAPIController.ManageResourceMasterStatus(inputDTO);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
