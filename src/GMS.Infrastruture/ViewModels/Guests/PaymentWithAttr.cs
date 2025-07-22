using GMS.Infrastructure.Models.Guests;

namespace GMS.Infrastructure.ViewModels.Guests
{
    public class PaymentWithAttr : PaymentDTO
    {

        public string? PaymentMethodName { get; set; }
        public string? GuestName { get; set; }
        public string? UHID { get; set; }
        public string? ApprovedByName { get; set; }
    }
}
