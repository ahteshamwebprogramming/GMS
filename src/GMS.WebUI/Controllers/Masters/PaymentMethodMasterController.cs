using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Masters;

public class PaymentMethodMasterController : Controller
{
    private readonly ILogger<PaymentMethodMasterController> _logger;
    private readonly PaymentMethodAPIController _paymentMethodAPIController;

    public PaymentMethodMasterController(ILogger<PaymentMethodMasterController> logger, PaymentMethodAPIController paymentMethodAPIController)
    {
        _logger = logger;
        _paymentMethodAPIController = paymentMethodAPIController;
    }
    public async Task<IActionResult> List()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Save(PaymentMethodDTO dataVM)
    {
        if (dataVM != null)
        {
            if (dataVM.Id == 0)
            {
                dataVM.IsActive = true;
                dataVM.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                dataVM.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var res = await _paymentMethodAPIController.Add(dataVM);
                return res;
            }
            else
            {
                var res = await _paymentMethodAPIController.Update(dataVM);
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
        PaymentMethodViewModel dto = new PaymentMethodViewModel();

        var res = await _paymentMethodAPIController.List();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.PaymentMethods = (List<PaymentMethodDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_paymentMethodMasterList/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] PaymentMethodDTO inputDTO)
    {
        PaymentMethodViewModel viewModel = new PaymentMethodViewModel();
        if (inputDTO.Id > 0)
        {
            var res = await _paymentMethodAPIController.PaymentMethodById(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.PaymentMethod = (PaymentMethodDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_paymentMethodMasterList/_add", viewModel);
    }
    public async Task<IActionResult> Delete([FromBody] PaymentMethodDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _paymentMethodAPIController.DeletePaymentMethod(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
