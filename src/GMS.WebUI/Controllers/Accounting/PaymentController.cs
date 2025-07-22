using Accounting.Controllers;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Accounting;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.ResourceAllocation;
using GMS.WebUI.Controllers.ResourceAllocation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Accounting;

public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;
    private readonly PaymentAPIController _paymentAPIController;
    public PaymentController(ILogger<PaymentController> logger, PaymentAPIController paymentAPIController)
    {
        _logger = logger;
        _paymentAPIController = paymentAPIController;
    }
    public IActionResult List()
    {
        return View();
    }

    public async Task<IActionResult> ListPartialView([FromBody] PaymentViewModel inputDTO)
    {
        //PaymentViewModel dto = new PaymentViewModel();
        var payments = await _paymentAPIController.GetPaymentsWithAttrMonthYearWise(inputDTO.Month ?? DateTime.Now.Month, inputDTO.Year ?? DateTime.Now.Year);
        if (payments != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)payments).StatusCode == 200)
        {
            inputDTO.PaymentsWithAttr = (List<PaymentWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)payments).Value;
        }
        return PartialView("_list/_payment", inputDTO);
    }
    public async Task<IActionResult> ApprovePayment([FromBody] PaymentDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));
            inputDTO.ApprovedBy = loginId;
            inputDTO.ApprovalDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var res = await _paymentAPIController.ApprovePayment(inputDTO);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }
}
