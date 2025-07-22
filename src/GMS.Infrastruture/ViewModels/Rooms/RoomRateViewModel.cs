using GMS.Infrastructure.Models.Rooms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomRateViewModel
    {
        public int? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public List<RatesDTO>? DailyRates { get; set; } = new List<RatesDTO>();
        public int? TotallRooms { get; set; }
    }
}
