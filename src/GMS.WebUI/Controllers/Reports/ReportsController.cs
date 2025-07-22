using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.ViewModels.Dashboard;
using GMS.Infrastructure.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Reports
{
    public class ReportsController : Controller
    {
        private readonly DashboardAPIController _dashboardAPIController;
        private readonly ILogger<ReportsController> _logger;
        public ReportsController(ILogger<ReportsController> logger, DashboardAPIController dashboardAPIController)
        {
            _logger = logger;
            _dashboardAPIController = dashboardAPIController;
        }
        public IActionResult SummaryReport()
        {
            return View();
        }
        public IActionResult FeedbackSummary()
        {
            return View();
        }
        public IActionResult FeedbackData()
        {
            return View();
        }
        public IActionResult RoomBookingStatus()
        {
            return View();
        }
        public async Task<IActionResult> GuestExperienceKPIs(int? month, int? year)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);
            DashboardViewModel dto = new DashboardViewModel();
            var res = await _dashboardAPIController.GetDashboardRoomOccupancyData(selectedDate);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomOccupancyDataList = (List<RoomOccupancyData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> MarketingAndDigitalKPIs(int? month, int? year)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);

            DashboardViewModel dto = new DashboardViewModel();
            var res = await _dashboardAPIController.GetDashboardRoomOccupancyData(selectedDate);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomOccupancyDataList = (List<RoomOccupancyData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> OperationalKPIs(int? month, int? year)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);

            DashboardViewModel dto = new DashboardViewModel();
            var res = await _dashboardAPIController.GetDashboardRoomOccupancyData(selectedDate);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomOccupancyDataList = (List<RoomOccupancyData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> FinancialKPIs(int? month, int? year)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);
            FinancialKPIViewModel dto = new FinancialKPIViewModel();
            var res = await _dashboardAPIController.GetFinancialKPIData(selectedDate);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.FinancialKPIDatas = (List<FinancialKPIData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            dto.Date = selectedDate;
            return View(dto);
        }
        public async Task<IActionResult> AdvanceAnalytics(int? month, int? year)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            DateTime selectedDate = new DateTime(year.Value, month.Value, 1);
            DashboardViewModel dto = new DashboardViewModel();
            var res = await _dashboardAPIController.GetDashboardRoomOccupancyData(selectedDate);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomOccupancyDataList = (List<RoomOccupancyData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(dto);
        }
    }
}
