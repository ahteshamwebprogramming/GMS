using System.Collections.Generic;

namespace GMS.Infrastructure.Models.Rooms
{
    public class RoomChkListDTO
    {
        public int Id { get; set; }

        public int? RID { get; set; }

        public List<int>? RoomIds { get; set; }

        public string? RChkLstID { get; set; }

        public DateTime? ChkDate { get; set; }

        public int? CheckedBy { get; set; }
        public string? CheckedByName { get; set; }

        public string? Reason { get; set; }

        public string? Comments { get; set; }
    }
}
