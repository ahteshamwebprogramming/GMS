namespace GMS.Infrastructure.ViewModels.Reports
{
    public class RoomRevenueData
    {
        public DateTime? TheDate { get; set; }
        public double? TotalRevenue { get; set; }
        public double? MTDRevenue { get; set; }
        public double? YTDRevenue { get; set; }
        public double? CPD_Change { get; set; }
        public double? CPM_Change { get; set; }
        public double? CPY_Change { get; set; }
    }
    public class RevenueData
    {
        public DateTime? TheDate { get; set; }
        public double? TotalRevenue { get; set; }
        public double? MTDRevenue { get; set; }
        public double? YTDRevenue { get; set; }
        public double? CPD_Change { get; set; }
        public double? CPM_Change { get; set; }
        public double? CPY_Change { get; set; }
    }
    public class RevenueDataADRREVPARPERIODWISE
    {
        public string? Period { get; set; }
        public double? TotalRevenue { get; set; }
        public double? TotalBookedRooms { get; set; }
        public double? TotalRooms { get; set; }
        public double? ADR { get; set; }
        public double? REVPAR { get; set; }
    }
    public class Result
    {
        public double? Value { get; set; }
    }
}
