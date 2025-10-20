using GMS.Infrastructure.ViewModels.Reports;

namespace GMS.Infrastructure.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public List<RoomOccupancyData>? RoomOccupancyDataList { get; set; }
        public List<RoomRevenueData>? RoomRevenueDataList { get; set; }
        //public List<RevenueData>? FnBRevenueDataList { get; set; }
        public List<RevenueData>? PackageRevenueDataList { get; set; }
        //public List<RevenueData>? ServiceDataList { get; set; }
        public List<RevenueData>? UpsellRevenueDataList { get; set; }
        public List<RevenueData>? PaymentDataList { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARToday { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARYesterday { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARMTD { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARYTD { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARCPD { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARCPM { get; set; }
        public RevenueDataADRREVPARPERIODWISE? ADRREVPARCPY { get; set; }
        public Result? AverageSellingRate { get; set; }        
        public Result? AverageSellingRateOverall { get; set; }        
        public Result? AverageOccupancyYearly { get; set; }        
        public Result? AverageSellingRoomsRateOverall_Audit { get; set; }        
        public Result? AverageSellingPackagesRateOverall_Audit { get; set; }        
        public DateTime? Date { get; set; }        
    }
}
