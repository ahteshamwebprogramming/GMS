using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Reports
{
    public class ReportsController : Controller
    {
        public IActionResult SummaryReport()
        {
            return View();
        }public IActionResult FeedbackSummary()
        {
            return View();
        }public IActionResult FeedbackData()
        {
            return View();
        }public IActionResult RoomBookingStatus()
        {
            return View();
        }
    }
}
