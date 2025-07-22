using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Masters;

public class CheckListController : Controller
{
    private readonly ILogger<CheckListController> _logger;
    private readonly CheckListAPIController _checkListAPIController;
    public CheckListController(ILogger<CheckListController> logger, CheckListAPIController checkListAPIController)
    {
        _logger = logger;
        _checkListAPIController = checkListAPIController;
    }
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
        CheckListViewModel dto = new CheckListViewModel();
        if (inputDTO != null)
        {
            var res = await _checkListAPIController.CheckInList(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.TblCheckListss = (List<TblCheckListsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            if (inputDTO.CheckListType == "CheckIn")
            {
                return PartialView("_checkInList/_list", dto);
            }
            else if (inputDTO.CheckListType == "CheckOut")
            {
                return PartialView("_checkOutList/_list", dto);
            }
        }
        return PartialView("_checkOutList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] TblCheckListsDTO inputDTO)
    {
        CheckListViewModel dto = new CheckListViewModel();
        if (inputDTO != null)
        {
            if (inputDTO.ID > 0)
            {
                var res = await _checkListAPIController.CheckListById(inputDTO.ID);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.TblCheckLists = (TblCheckListsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            if (inputDTO.ChecklistType == "CheckIn")
            {
                return PartialView("_checkInList/_add", dto);
            }
            else if (inputDTO.ChecklistType == "CheckOut")
            {
                return PartialView("_checkOutList/_add", dto);
            }
        }
        return PartialView("_checkInList/_add", dto);
    }
    [HttpPost]
    public async Task<IActionResult> Save(TblCheckListsDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.ID == 0)
            {
                dataVM.IsActive = true;
                dataVM.Type = 0;
                dataVM.ChkIn = 0;
                dataVM.ChkOut = 0;
                //dataVM.CreatedDate = DateTime.Now;
                //dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _checkListAPIController.Add(dataVM);
                return res;
            }
            else
            {
                //dataVM.ModifiedDate = DateTime.Now;
                //dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _checkListAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> Delete([FromBody] TblCheckListsDTO inputDTO)
    {
        if (inputDTO.ID > 0)
        {
            var res = await _checkListAPIController.Delete(inputDTO.ID);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }



    #region RoomCheckList

    public IActionResult RoomCleaningCheckList()
    {
        return View();
    }
    public async Task<IActionResult> RoomCleaningCheckListPartialView([FromBody] CheckListViewModel inputDTO)
    {
        CheckListViewModel dto = new CheckListViewModel();
        if (inputDTO != null)
        {
            var res = await _checkListAPIController.CheckInList(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.TblCheckListss = (List<TblCheckListsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }            
        }
        return PartialView("_roomCleaningCheckList/_list", dto);
    }
    public async Task<IActionResult> RoomCleaningCheckListAddPartialView([FromBody] TblCheckListsDTO inputDTO)
    {
        CheckListViewModel dto = new CheckListViewModel();
        if (inputDTO != null)
        {
            if (inputDTO.ID > 0)
            {
                var res = await _checkListAPIController.CheckListById(inputDTO.ID);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.TblCheckLists = (TblCheckListsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }            
        }
        return PartialView("_roomCleaningCheckList/_add", dto);
    }
    [HttpPost]
    public async Task<IActionResult> RoomCleaningCheckListSave(TblCheckListsDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.ID == 0)
            {
                dataVM.IsActive = true;
                dataVM.Type = 0;
                dataVM.ChkIn = 0;
                dataVM.ChkOut = 0;
                //dataVM.CreatedDate = DateTime.Now;
                //dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _checkListAPIController.Add(dataVM);
                return res;
            }
            else
            {
                //dataVM.ModifiedDate = DateTime.Now;
                //dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                var res = await _checkListAPIController.Update(dataVM);
                return res;
            }
        }
        else
        {
            return BadRequest("Data is not valid");
        }
        return null;
    }
    public async Task<IActionResult> RoomCleaningCheckListDelete([FromBody] TblCheckListsDTO inputDTO)
    {
        if (inputDTO.ID > 0)
        {
            var res = await _checkListAPIController.Delete(inputDTO.ID);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
    #endregion
}
