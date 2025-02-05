using GMS.Infrastructure.Models.Guests;

namespace GMS.Infrastructure.ViewModels.Guests;

public class AddRoomPopupViewModel
{
    public MembersDetailsDTO? MembersDetails { get; set; }
    public AvailableRoomsSharedStatus? AvailableRoomsSharedStatus { get; set; }
    public List<AvailableRoomsForGuestAllocation>? AvailableRoomsForGuestAllocationList { get; set; }
}
