using GMS.Infrastructure.Models.Guests;

namespace GMS.Infrastructure.ViewModels.CRM
{
    public class EnquiryListViewModel
    {
        public List<GuestReservationDTO>? GuestEnquiryList { get; set; }
        public List<GuestReservationWithAttr>? GuestEnquiryWithAttrList { get; set; }
        public GuestReservationDTO? GuestEnquiry { get; set; }
    }
}
