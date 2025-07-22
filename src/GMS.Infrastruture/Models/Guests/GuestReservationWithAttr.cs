namespace GMS.Infrastructure.Models.Guests
{
    public class GuestReservationWithAttr : GuestReservationDTO
    {
        public string? RatePlan { get; set; }
        public string? RoomTypeName { get; set; }
        public string? SaleSourceName { get; set; }
        public string? LeadSourceName { get; set; }
        public string? BrandAwarenessName { get; set; }
    }
}
