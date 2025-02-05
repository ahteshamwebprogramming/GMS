using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters
{
    public class CheckListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CheckInList()
        {
            return View();
        }
        public IActionResult CheckOutList()
        {
            return View();
        }
        public async Task<IActionResult> ListPartialView([FromBody] CheckListViewModel inputDTO)
        {
            RoomTypeViewModel dto = new RoomTypeViewModel();

            var res = await _roomTypeAPIController.List();
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return PartialView("_roomTypeList/_list", dto);
        }

    }
}
