using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters;

public class RoomTypeViewModel
{
    public RoomTypeDTO? RoomType { get; set; }
    public List<RoomTypeDTO>? RoomTypes { get; set; }

    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public int TotalRooms { get; set; }
    public int RoomsAvailable { get; set; }
}
