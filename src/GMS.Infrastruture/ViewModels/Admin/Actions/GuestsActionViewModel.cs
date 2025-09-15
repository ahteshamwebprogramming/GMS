using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Admin.Actions
{
    public class GuestsActionViewModel
    {
        public string? SearchField { get; set; }
        public int? GuestId { get; set; }
        public List<MembersDetailsDTO>? GuestsList { get; set; }
        public List<RoomAllocationDTO>? RoomAllocationList { get; set; }
        public List<BillingDTO>? BillingList { get; set; }
        public List<PaymentDTO>? PaymentList { get; set; }
        public List<SettlementDTO>? SettlementList { get; set; }
    }
}
