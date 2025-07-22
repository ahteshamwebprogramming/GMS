namespace GMS.Infrastructure.ViewModels.Rooms;

public class RoomAvailabilityDTO
{
    public int ID { get; set; }
    public string? RNumber { get; set; }
    public string? RType { get; set; }
    public string? AvailabilityColumn { get; set; }
    public DateTime? DateValue { get; set; }
    public int? NoOfRooms { get; set; }
}
