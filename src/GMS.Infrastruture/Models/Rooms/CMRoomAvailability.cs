namespace GMS.Infrastructure.Models.Rooms
{
    public class CMRoomAvailability
    {
        public int? RtypeID { get; set; }
        public string? Name { get; set; }
        public int? TotalRooms { get; set; }        
    }
    public class CMOccupancyTable
    {
        public int RtypeID { get; set; }
        public DateTime OccupancyDate { get; set; }
        public int BookedRooms { get; set; }        
    }
}
