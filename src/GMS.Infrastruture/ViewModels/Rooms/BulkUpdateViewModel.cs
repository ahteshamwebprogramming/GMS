using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class BulkUpdateViewModel
    {
        public List<ServicesDTO>? RatePlansList { get; set; }
        public int ChannelId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<RoomRateBulkUpdate> Rates { get; set; }
        public int Days { get; set; }
        public List<RoomInventoryBulkUpdate> Inventory { get; set; }
        public DateTime StartDate { get; set; }
        public List<RoomRestrictionBulkUpdate> Restrictions { get; set; }
        public List<RoomPercentage> Percentages { get; set; }
    }
    public class RoomPercentage
    {
        public int RoomTypeId { get; set; }
        public decimal[] Percentages { get; set; }
        public bool IsSelected { get; set; }
    }
    public class RoomRateBulkUpdate
    {
        public int RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? MinimumRate { get; set; }
        public decimal? MaximumRate { get; set; }
        public bool IsSelected { get; set; }
    }
    public class RoomInventoryBulkUpdate
    {
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }

        public int? RoomsAvailable { get; set; }
        public int? RoomsOpen { get; set; }
        public bool IsSelected { get; set; }

    }
    public class RoomRestrictionBulkUpdate
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public bool? StopSell { get; set; }
        public bool? CloseOnArrival { get; set; }
        public bool? RestrictStay { get; set; }
        public int? MinimumNights { get; set; }
        public int? MaximumNights { get; set; }

        public bool IsSelected { get; set; }
    }

}
