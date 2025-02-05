using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Rooms;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class AmenitiesController : Controller
{
    private readonly ILogger<AmenitiesController> _logger;
    private readonly AmenitiesAPIController _amenitiesAPIController;
    private readonly AmenetiesCategoryAPIController _amenetiesCategoryAPIController;

    public AmenitiesController(ILogger<AmenitiesController> logger, AmenitiesAPIController amenitiesAPIController, AmenetiesCategoryAPIController amenetiesCategoryAPIController)
    {
        _logger = logger;
        _amenitiesAPIController = amenitiesAPIController;
        _amenetiesCategoryAPIController = amenetiesCategoryAPIController;
    }
    public async Task<IActionResult> List()
    {
        //RoomTypeViewModel dto = new RoomTypeViewModel();

        //var res = await _amenitiesAPIController.List();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto.Amenitiess = (List<AmenitiesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(AmenitiesDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                var res = await _amenitiesAPIController.Add(dataVM);
                return res;
            }
            else
            {
                var res = await _amenitiesAPIController.Update(dataVM);
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
        AmenitiesViewModel dto = new AmenitiesViewModel();



        var res = await _amenitiesAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Amenitiess = (List<AmenitiesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_amenitiesList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] AmenitiesDTO inputDTO)
    {
        AmenitiesViewModel viewModel = new AmenitiesViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _amenitiesAPIController.AmenitiesById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.Amenities = (AmenitiesDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }

        var resAmenetiesCategory = await _amenetiesCategoryAPIController.List();
        if (resAmenetiesCategory != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAmenetiesCategory).StatusCode == 200)
        {
            viewModel.AmenetiesCategories = (List<AmenetiesCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAmenetiesCategory).Value;
        }

        return PartialView("_amenitiesList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] AmenitiesDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _amenitiesAPIController.DeleteAmenities(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }

}
