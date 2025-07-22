namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomTypePercentageEntity
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Percentage { get; set; }
        public byte PercentageSlot { get; set; }
    }
}
