namespace GMS.Infrastructure.ViewModels.Dashboard
{
    public class RoomOccupancyData
    {
        public DateTime? TheDate { get; set; }
        public int? ID { get; set; }
        public string? RType { get; set; }
        public int? TotalRooms { get; set; }
        public int? BookedRooms { get; set; }
        public int? AvailableRooms { get; set; }
        public decimal? PercentOccupied { get; set; }
        public decimal? TodayCheckOuts { get; set; }
        public decimal? TodayCheckIns { get; set; }
        public decimal? TidyRooms { get; set; }
    }
}
