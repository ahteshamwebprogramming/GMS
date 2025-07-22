using Accounting.Controllers;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Accounting;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Accounting;

public class InvoicesController : Controller
{

    private readonly ILogger<InvoicesController> _logger;
    private readonly InvoicesAPIController _invoicesAPIController;
    public InvoicesController(ILogger<InvoicesController> logger, InvoicesAPIController invoicesAPIController)
    {
        _logger = logger;
        _invoicesAPIController = invoicesAPIController;
    }
    public IActionResult List()
    {
        return View();
    }
    public async Task<IActionResult> ListPartialView([FromBody] InvoicesViewModel inputDTO)
    {
        //InvoicesViewModel dto = new InvoicesViewModel();

        var invoicing = await _invoicesAPIController.GetInvoicesWithAttrMonthYearWise(inputDTO.Month ?? DateTime.Now.Month, inputDTO.Year ?? DateTime.Now.Year);
        if (invoicing != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)invoicing).StatusCode == 200)
        {
            inputDTO.Invoicing = (List<InvoicingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)invoicing).Value;
        }
        return PartialView("_list/_invoices", inputDTO);
    }
    public async Task<IActionResult> ApproveInvoices([FromBody] SettlementDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));
            inputDTO.ApprovedBy = loginId;
            inputDTO.ApprovedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var res = await _invoicesAPIController.ApproveInvoices(inputDTO);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }
}
