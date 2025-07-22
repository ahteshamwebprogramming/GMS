namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomRestrictionViewModel
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public List<RoomRestriction> DailyRestrictions { get; set; }
    }
}
