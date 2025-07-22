using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class PackageMasterController : Controller
{
    private readonly ILogger<PackageMasterController> _logger;
    private readonly ServicesAPIController _servicesAPIController;

    public PackageMasterController(ILogger<PackageMasterController> logger, ServicesAPIController servicesAPIController)
    {
        _logger = logger;
        _servicesAPIController = servicesAPIController;
    }
    public async Task<IActionResult> List()
    {
        //RoomTypeViewModel dto = new RoomTypeViewModel();

        //var res = await _servicesAPIController.List();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(ServicesDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.Status = 1;
                dataVM.Readonly = false;
                var res = await _servicesAPIController.Add(dataVM);
                return res;
            }
            else
            {
                var res = await _servicesAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> ListPartialView()
    {
        PackageMastersViewModel dto = new PackageMastersViewModel();

        var res = await _servicesAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_packageMasterList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] ServicesDTO inputDTO)
    {
        PackageMastersViewModel viewModel = new PackageMastersViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _servicesAPIController.ServiceById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.Service = (ServicesDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_packageMasterList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] ServicesDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _servicesAPIController.DeleteServices(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }

}
