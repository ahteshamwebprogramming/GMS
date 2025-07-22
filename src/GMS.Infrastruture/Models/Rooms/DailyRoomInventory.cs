namespace GMS.Infrastructure.Models.Rooms
{
    public class DailyRoomInventory
    {
        public int RoomTypeId;

        public int Id { get; set; }
        public int RtypeID { get; set; }
        public DateTime Date { get; set; }
        public int TotalRooms { get; set; }
        public int RoomsAvailable { get; set; }
        public int TotalRoomForSale { get; set; }
        public int BookedRooms { get; set; }
        public decimal OccupancyPercentage => TotalRooms > 0 ?
        ((decimal)(TotalRooms - RoomsAvailable) / TotalRooms) * 100 : 0;
    }
}
