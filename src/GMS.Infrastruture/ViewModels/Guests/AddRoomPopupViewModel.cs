using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Guests;

public class AddRoomPopupViewModel
{
    public MembersDetailsDTO? MembersDetails { get; set; }
    public MemberDetailsWithChild? MembersDetailWithChild { get; set; }
    public AvailableRoomsSharedStatus? AvailableRoomsSharedStatus { get; set; }
    public List<AvailableRoomsForGuestAllocation>? AvailableRoomsForGuestAllocationList { get; set; }
    public RoomAllocationDTO? RoomAllocationDetails { get; set; }

    public List<MembersDetailsDTO>? RoomPartnerName { get; set; }
    public bool isShared { get; set; }
    public List<RoomTypeDTO>? RoomTypes { get; set; }
}
