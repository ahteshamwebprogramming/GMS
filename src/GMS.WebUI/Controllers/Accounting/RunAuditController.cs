﻿using GMS.Endpoints.Accounting;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.ViewModels.Dashboard;
using GMS.Infrastructure.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Accounting;

public class RunAuditController : Controller
{
    private readonly ILogger<RunAuditController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly AuditAPIController _auditAPIController;
    private readonly DashboardAPIController _dashboardAPIController;

    public RunAuditController(ILogger<RunAuditController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, DashboardAPIController dashboardAPIController, AuditAPIController auditAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _dashboardAPIController = dashboardAPIController;
        _auditAPIController = auditAPIController;
    }
    public async Task<IActionResult> Index()
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


        return View(dto);

    }

    public async Task<IActionResult> RunNightAudit()
    {
        int loggedinUser = Convert.ToInt32(User.FindFirstValue("Id"));
        var res = await _auditAPIController.RunAudit(loggedinUser);
        return res;
    }
}
