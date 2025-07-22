namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomIncrementData
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int NumberOfRooms { get; set; }
        //public decimal[] Percentages { get; set; }
        public List<decimal> Percentages { get; set; }
    }
}
