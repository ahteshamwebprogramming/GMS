using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomStatusViewModel
    {
        public List<RoomsDTO>? Rooms { get; set; }
        public RoomsWithAttribute? RoomWithAttribute { get; set; }
        public List<TblCheckListsDTO>? RoomCleanCheckList { get; set; }
        public RoomChkListDTO? RoomCheckList { get; set; }

        public List<RoomsWithAttribute>? RoomWithAttributeList { get; set; }
    }
}
