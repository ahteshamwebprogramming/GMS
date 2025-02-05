using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.CRM;

public class CRMController : Controller
{
    public IActionResult NewEnquiry()
    {
        return View();
    }
    public IActionResult CallingList()
    {
        return View();
    }
    public IActionResult BookingList()
    {
        return View();
    }
    public IActionResult EnquiryList()
    {
        return View();
    }
}
