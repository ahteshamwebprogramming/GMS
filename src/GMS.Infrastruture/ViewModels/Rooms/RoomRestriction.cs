namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomRestriction
    {
        public int RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public bool? StopSell { get; set; }
        public bool? CloseOnArrival { get; set; }
        public bool? RestrictStay { get; set; }
        public int? MinimumNights { get; set; }
        public int? MaximumNights { get; set; }
    }
}
