using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class RoomTypeController : Controller
{
    private readonly ILogger<RoomTypeController> _logger;
    private readonly RoomTypeAPIController _roomTypeAPIController;

    public RoomTypeController(ILogger<RoomTypeController> logger, RoomTypeAPIController roomTypeAPIController)
    {
        _logger = logger;
        _roomTypeAPIController = roomTypeAPIController;
    }
    public async Task<IActionResult> List()
    {
        //RoomTypeViewModel dto = new RoomTypeViewModel();

        //var res = await _roomTypeAPIController.List();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(RoomTypeDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.Status = 1;                
                var res = await _roomTypeAPIController.Add(dataVM);
                return res;
            }
            else
            {                
                var res = await _roomTypeAPIController.Update(dataVM);
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
        RoomTypeViewModel dto = new RoomTypeViewModel();

        var res = await _roomTypeAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_roomTypeList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] RoomTypeDTO inputDTO)
    {
        RoomTypeViewModel viewModel = new RoomTypeViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _roomTypeAPIController.RoomTypeById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.RoomType = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_roomTypeList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] RoomTypeDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _roomTypeAPIController.DeleteRoomType(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
    
}
