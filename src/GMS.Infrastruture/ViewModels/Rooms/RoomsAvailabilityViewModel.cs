using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomsAvailabilityViewModel
    {
        public List<RoomAvailabilityDTO>? RoomAvailabilities { get; set; }
        public List<RoomLockDTO>? RoomLocks { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<MembersDetailsDTO>? MembersDetails { get; set; }
        public string? RoomNo { get; set; }
        public string? GroupId { get; set; }
    }
}
