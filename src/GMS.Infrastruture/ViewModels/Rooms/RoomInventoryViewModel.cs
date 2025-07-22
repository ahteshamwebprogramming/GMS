using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomInventoryViewModel
    {
        public int? RtypeID { get; set; }
        public int? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public List<DailyRoomInventory>? DailyInventory { get; set; }
    }
}
