namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomRatesForEnquiry
    {
        public int? RateId { get; set; }
        public int? RoomTypeId { get; set; }
        public DateTime? RateDate { get; set; }
        public double? Rate { get; set; }
        public int? PlanId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanDescription { get; set; }
        public string? RoomType { get; set; }
        public string? RoomDescription { get; set; }
        public int? NoOfNights { get; set; }
        public int? NoOfRooms { get; set; }
    }
}
