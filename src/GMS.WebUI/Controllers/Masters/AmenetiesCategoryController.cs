using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class AmenetiesCategoryController : Controller
{
    private readonly ILogger<AmenetiesCategoryController> _logger;
    private readonly AmenetiesCategoryAPIController _amenetiesCategoryAPIController;

    public AmenetiesCategoryController(ILogger<AmenetiesCategoryController> logger, AmenetiesCategoryAPIController amenetiesCategoryAPIController)
    {
        _logger = logger;
        _amenetiesCategoryAPIController = amenetiesCategoryAPIController;
    }
    public async Task<IActionResult> List()
    {
        //RoomTypeViewModel dto = new RoomTypeViewModel();

        //var res = await _amenetiesCategoryAPIController.List();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto.AmenetiesCategorys = (List<AmenetiesCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(AmenetiesCategoryDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;                
                var res = await _amenetiesCategoryAPIController.Add(dataVM);
                return res;
            }
            else
            {                
                var res = await _amenetiesCategoryAPIController.Update(dataVM);
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
        AmenetiesCategoryViewModel dto = new AmenetiesCategoryViewModel();

        var res = await _amenetiesCategoryAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.AmenetiesCategories = (List<AmenetiesCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_amenetiesCategoryList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] AmenetiesCategoryDTO inputDTO)
    {
        AmenetiesCategoryViewModel viewModel = new AmenetiesCategoryViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _amenetiesCategoryAPIController.AmenetiesCategoryById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.AmenetiesCategory = (AmenetiesCategoryDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_amenetiesCategoryList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] AmenetiesCategoryDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _amenetiesCategoryAPIController.DeleteAmenetiesCategory(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
    
}
