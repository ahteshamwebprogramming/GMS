using Accounting.Controllers;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Reports;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Reports;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Accounting;

public class AuditReportController : Controller
{
    private readonly ILogger<AuditReportController> _logger;
    private readonly SalesReportAPIController _salesReportAPIController;
    public AuditReportController(ILogger<AuditReportController> logger, SalesReportAPIController salesReportAPIController)
    {
        _logger = logger;
        _salesReportAPIController = salesReportAPIController;
    }
    public IActionResult List()
    {
        return View();
    }

    public async Task<IActionResult> GetServicesForBilling([FromBody] SalesReportViewModel inputDTO)
    {
        SalesReportViewModel dto = new SalesReportViewModel();
        var settlements = await _salesReportAPIController.GetAllSettlements(inputDTO);
        if (settlements != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)settlements).StatusCode == 200)
        {
            dto.Settlements = (List<SettlementDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)settlements).Value;
        }
        var auditRevenue = await _salesReportAPIController.GetAllAuditRevenue(inputDTO);
        if (auditRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)auditRevenue).StatusCode == 200)
        {
            dto.AuditedRevenues = (List<AuditedRevenueDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)auditRevenue).Value;
        }

        return Ok(dto);
    }
}
