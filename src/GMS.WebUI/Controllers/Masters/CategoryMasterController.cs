using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class CategoryMasterController : Controller
{
    private readonly ILogger<CategoryMasterController> _logger;
    private readonly CategoryMasterAPIController _categoryMasterAPIController;
    private readonly RoleMasterAPIController _roleMasterAPIController;

    public CategoryMasterController(ILogger<CategoryMasterController> logger, CategoryMasterAPIController categoryMasterAPIController, RoleMasterAPIController roleMasterAPIController)
    {
        _logger = logger;
        _categoryMasterAPIController = categoryMasterAPIController;
        _roleMasterAPIController = roleMasterAPIController;
    }
    public async Task<IActionResult> List()
    {
        //CategoryMasterViewModel dto = new CategoryMasterViewModel();

        //var res = await _categoryMasterAPIController.List();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto.CategoryMasters = (List<CategoryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(CategoryMasterDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsDeleted = false;
                dataVM.IsActive = true;
                dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                dataVM.CreatedDate = DateTime.Now;
                var res = await _categoryMasterAPIController.Add(dataVM);
                return res;
            }
            else
            {
                dataVM.ModifiedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                dataVM.ModifiedDate = DateTime.Now;
                var res = await _categoryMasterAPIController.Update(dataVM);
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
        CategoryMasterViewModel dto = new CategoryMasterViewModel();

        var res = await _categoryMasterAPIController.ListWithAttributes();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.CategoryMasterWithAttributes = (List<CategoryMasterWithAttributes>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_categoryMasterList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] CategoryMasterDTO inputDTO)
    {
        CategoryMasterViewModel viewModel = new CategoryMasterViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _categoryMasterAPIController.CategoryMasterById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.CategoryMaster = (CategoryMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        var resRoles = await _roleMasterAPIController.List();
        if (resRoles != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).StatusCode == 200)
        {
            viewModel.Roles = (List<RoleMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoles).Value;
        }
        return PartialView("_categoryMasterList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] CategoryMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _categoryMasterAPIController.DeleteCategoryMaster(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
    public async Task<IActionResult> GetCategoriesByDepartment([FromBody] CategoryMasterDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _categoryMasterAPIController.GetCategoriesByDepartment(inputDTO);
            return res;
        }
        return BadRequest("Invalid Request");
    }

    public async Task<IActionResult> ManageCategoryMasterStatus([FromBody] CategoryMasterDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _categoryMasterAPIController.ManageCategoryMasterStatus(inputDTO);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }

}
