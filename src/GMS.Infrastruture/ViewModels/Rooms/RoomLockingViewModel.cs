using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms;

public class RoomLockingViewModel
{
    public List<RoomsDTO>? RoomsList { get; set; }
    public RoomLockDTO? RoomLock { get; set; }
    public List<RoomLockDTO>? RoomLockList { get; set; }
    public List<RoomsWithStatusDTO>? RoomsWithStatuses { get; set; }
    public RoomLockDAO? RoomLockDAO { get; set; }
    public List<RoomTypeDTO>? RoomTypes { get; set; }
    public List<AmenitiesDTO>? Amenities { get; set; }
    public List<RoomAmenetiesDTO>? RoomAmenitiesAssigned { get; set; }
    public List<RoomAmenetiesWithChild>? RoomAmenitiesAssignedWithChild { get; set; }
    public List<RoomsPicturesDTO>? RoomImages { get; set; }
    public List<IFormFile>? Attachments { get; set; }
    public RoomsDTO? Room { get; set; }
    public int[]? AmenityIds { get; set; }
}
