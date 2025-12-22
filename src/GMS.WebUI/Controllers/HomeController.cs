using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Dashboard;
using GMS.Infrastructure.ViewModels.Home;
using GMS.Infrastructure.ViewModels.Reports;
using GMS.Infrastructure.ViewModels.Wellness;
using GMS.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GMS.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GuestsAPIController _guestsAPIController;
        private readonly AccountsAPIController _accountsAPIController;
        private readonly DashboardAPIController _dashboardAPIController;
        private readonly GMS.Endpoints.Guests.TasksAssignedAPIController _tasksAssignedAPIController;

        public HomeController(ILogger<HomeController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, DashboardAPIController dashboardAPIController, GMS.Endpoints.Guests.TasksAssignedAPIController tasksAssignedAPIController)
        {
            _logger = logger;
            _guestsAPIController = guestsAPIController;
            _accountsAPIController = accountsAPIController;
            _dashboardAPIController = dashboardAPIController;
            _tasksAssignedAPIController = tasksAssignedAPIController;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> WellnessStatusBoard(DateTime? boardDate)
        {
            var targetDate = boardDate ?? DateTime.Today;
            var viewModel = new WellnessStatusBoardViewModel { BoardDate = targetDate };

            var res = await _dashboardAPIController.GetWellnessStatusBoardData(targetDate);
            if (res != null && res is OkObjectResult okResult)
            {
                viewModel.Schedules = (List<WellnessScheduleRow>?)okResult.Value ?? new List<WellnessScheduleRow>();
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetWellnessStatusBoardData(DateTime? boardDate)
        {
            var targetDate = boardDate ?? DateTime.Today;
            var res = await _dashboardAPIController.GetWellnessStatusBoardData(targetDate);
            if (res != null && res is OkObjectResult okResult)
            {
                return Json(okResult.Value);
            }
            return Json(new List<WellnessScheduleRow>());
        }

        public async Task<IActionResult> Default()
        {
            DashboardViewModel dto = new DashboardViewModel();
            var res = await _dashboardAPIController.GetDashboardRoomOccupancyDataCurrentDate();
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomOccupancyDataList = (List<RoomOccupancyData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            var resRoomRevenue = await _dashboardAPIController.GetRoomRevenueDataCurrentDate();
            if (resRoomRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).StatusCode == 200)
            {
                dto.RoomRevenueDataList = (List<RoomRevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).Value;
            }
            var resFnBRevenue = await _dashboardAPIController.GetPackagesDataCurrentDate();
            if (resFnBRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).StatusCode == 200)
            {
                dto.PackageRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).Value;
            }
            var resUpsellRevenue = await _dashboardAPIController.GetUpsellRevenueDataCurrentDate();
            if (resUpsellRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).StatusCode == 200)
            {
                dto.UpsellRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).Value;
            }
            var resPaymentData = await _dashboardAPIController.GetPaymentDataCurrentDate();
            if (resPaymentData != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resPaymentData).StatusCode == 200)
            {
                dto.PaymentDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPaymentData).Value;
            }
            var resADR_REVPAR_Today_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("TODAY");
            if (resADR_REVPAR_Today_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_Today_Data).StatusCode == 200)
            {
                dto.ADRREVPARToday = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_Today_Data).Value;
            }
            var resADR_REVPAR_Yesterday_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("YESTERDAY");
            if (resADR_REVPAR_Yesterday_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_Yesterday_Data).StatusCode == 200)
            {
                dto.ADRREVPARYesterday = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_Yesterday_Data).Value;
            }
            var resADR_REVPAR_MTD_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("MTD");
            if (resADR_REVPAR_MTD_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_MTD_Data).StatusCode == 200)
            {
                dto.ADRREVPARMTD = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_MTD_Data).Value;
            }
            var resADR_REVPAR_YTD_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("YTD");
            if (resADR_REVPAR_YTD_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_YTD_Data).StatusCode == 200)
            {
                dto.ADRREVPARYTD = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_YTD_Data).Value;
            }
            var resADR_REVPAR_CPD_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("CPD");
            if (resADR_REVPAR_CPD_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPD_Data).StatusCode == 200)
            {
                dto.ADRREVPARCPD = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPD_Data).Value;
            }
            var resADR_REVPAR_CPM_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("CPM");
            if (resADR_REVPAR_CPM_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPM_Data).StatusCode == 200)
            {
                dto.ADRREVPARCPM = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPM_Data).Value;
            }
            var resADR_REVPAR_CPY_Data = await _dashboardAPIController.GetADRREVPARPERIODWISE("CPY");
            if (resADR_REVPAR_CPY_Data != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPY_Data).StatusCode == 200)
            {
                dto.ADRREVPARCPY = (RevenueDataADRREVPARPERIODWISE?)((Microsoft.AspNetCore.Mvc.ObjectResult)resADR_REVPAR_CPY_Data).Value;
            }
            var resAverageSellingRate = await _dashboardAPIController.GetAverageSellingRate();
            if (resAverageSellingRate != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRate).StatusCode == 200)
            {
                dto.AverageSellingRate = (Result?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRate).Value;
            }
            var resAverageSellingRateOverall = await _dashboardAPIController.GetAverageSellingRateOverall();
            if (resAverageSellingRateOverall != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRateOverall).StatusCode == 200)
            {
                dto.AverageSellingRateOverall = (Result?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRateOverall).Value;
            }
            var resAverageSellingRoomsRateOverall_Audit = await _dashboardAPIController.GetAverageSellingRate_Rooms_Audit();
            if (resAverageSellingRoomsRateOverall_Audit != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRoomsRateOverall_Audit).StatusCode == 200)
            {
                dto.AverageSellingRoomsRateOverall_Audit = (Result?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingRoomsRateOverall_Audit).Value;
            }
            var resAverageSellingPackegsRateOverall_Audit = await _dashboardAPIController.GetAverageSellingRate_Packages_Audit();
            if (resAverageSellingPackegsRateOverall_Audit != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingPackegsRateOverall_Audit).StatusCode == 200)
            {
                dto.AverageSellingPackagesRateOverall_Audit = (Result?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageSellingPackegsRateOverall_Audit).Value;
            }
            var resAverageOccupancyYearly = await _dashboardAPIController.AverageOccupancYearly();
            if (resAverageOccupancyYearly != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageOccupancyYearly).StatusCode == 200)
            {
                dto.AverageOccupancyYearly = (Result?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageOccupancyYearly).Value;
            }
            return View(dto);
        }
        public async Task<IActionResult> Default1(int? month, int? year)
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
            var resRoomRevenue = await _dashboardAPIController.GetRoomRevenueData(selectedDate);
            if (resRoomRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).StatusCode == 200)
            {
                dto.RoomRevenueDataList = (List<RoomRevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).Value;
            }
            var resFnBRevenue = await _dashboardAPIController.GetPackageRevenueData(selectedDate);
            if (resFnBRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).StatusCode == 200)
            {
                dto.PackageRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).Value;
            }
            var resUpsellRevenue = await _dashboardAPIController.GetUpsellRevenueData(selectedDate);
            if (resUpsellRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).StatusCode == 200)
            {
                dto.UpsellRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).Value;
            }
            return View(dto);
        }

        public async Task<IActionResult> Dashboard(int? month, int? year)
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
            var resRoomRevenue = await _dashboardAPIController.GetRoomRevenueData(selectedDate);
            if (resRoomRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).StatusCode == 200)
            {
                dto.RoomRevenueDataList = (List<RoomRevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).Value;
            }
            var packageRevenue = await _dashboardAPIController.GetPackageRevenueData(selectedDate);
            if (packageRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)packageRevenue).StatusCode == 200)
            {
                dto.PackageRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)packageRevenue).Value;
            }
            var resUpsellRevenue = await _dashboardAPIController.GetUpsellRevenueData(selectedDate);
            if (resUpsellRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).StatusCode == 200)
            {
                dto.UpsellRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).Value;
            }
            dto.Date = selectedDate;
            return View(dto);
        }

        public async Task<IActionResult> GetDashboardChartRevenueVsOccupancy(int? month, int? year)
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
            var resRoomRevenue = await _dashboardAPIController.GetRoomRevenueData(selectedDate);
            if (resRoomRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).StatusCode == 200)
            {
                dto.RoomRevenueDataList = (List<RoomRevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRoomRevenue).Value;
            }
            var resFnBRevenue = await _dashboardAPIController.GetPackageRevenueData(selectedDate);
            if (resFnBRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).StatusCode == 200)
            {
                dto.PackageRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resFnBRevenue).Value;
            }
            var resUpsellRevenue = await _dashboardAPIController.GetUpsellRevenueData(selectedDate);
            if (resUpsellRevenue != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).StatusCode == 200)
            {
                dto.UpsellRevenueDataList = (List<RevenueData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resUpsellRevenue).Value;
            }
            return Ok(dto);
        }
        public async Task<IActionResult> Index1()
        {
            var res = await _guestsAPIController.GetGuestsList();

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                List<GMSFinalGuestDTO>? dto = (List<GMSFinalGuestDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return View(dto);
            }
            return View();
        }
        public async Task<IActionResult> EHRMSLogin()
        {
            var res = await _accountsAPIController.GetLoginList();

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                List<EHRMSLoginDTO>? dto = (List<EHRMSLoginDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return View(dto);
            }
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SendFeedback()
        {
            return View();
        }
        public IActionResult ReviewFeedback()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> TasksAssigned(DateTime? workDate, int? employeeId)
        {
            // Get worker ID from claims
            var workerIdClaim = User.FindFirstValue("WorkerId");
            if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var loggedInWorkerId) || loggedInWorkerId <= 0)
            {
                _logger.LogWarning("TasksAssigned requested without a valid WorkerId claim.");
                return RedirectToAction("Login", "Account");
            }

            // Use selected employee or default to logged-in worker
            var workerId = employeeId ?? loggedInWorkerId;

            // Default to today if no date provided
            var targetDate = workDate?.Date ?? DateTime.Today;

            // Fetch tasks assigned view model from API
            var res = await _tasksAssignedAPIController.GetTasksAssignedViewModel(targetDate, workerId, loggedInWorkerId);
            if (res != null && res is OkObjectResult okResult)
            {
                var viewModel = (TasksAssignedViewModel?)okResult.Value;
                if (viewModel != null)
                {
                    return View(viewModel);
                }
            }

            // Return empty view model on error
            return View(new TasksAssignedViewModel
            {
                WorkDate = targetDate,
                WorkerName = "Employee",
                Assignments = new List<GMS.Infrastructure.ViewModels.Rooms.HousekeepingAssignmentRow>()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksAssignedGrid(DateTime? workDate, int? employeeId)
        {
            // Get worker ID from claims
            var workerIdClaim = User.FindFirstValue("WorkerId");
            if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var loggedInWorkerId) || loggedInWorkerId <= 0)
            {
                return Unauthorized();
            }

            // Use selected employee or default to logged-in worker
            var workerId = employeeId ?? loggedInWorkerId;

            // Default to today if no date provided
            var targetDate = workDate?.Date ?? DateTime.Today;

            // Fetch tasks assigned view model from API
            var res = await _tasksAssignedAPIController.GetTasksAssignedViewModel(targetDate, workerId, loggedInWorkerId);
            if (res != null && res is OkObjectResult okResult)
            {
                var viewModel = (TasksAssignedViewModel?)okResult.Value;
                if (viewModel != null)
                {
                    return PartialView("_TasksAssignedGrid", viewModel);
                }
            }

            // Return empty view model on error
            return PartialView("_TasksAssignedGrid", new TasksAssignedViewModel
            {
                WorkDate = targetDate,
                WorkerName = "Employee",
                Assignments = new List<GMS.Infrastructure.ViewModels.Rooms.HousekeepingAssignmentRow>()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
