using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;

namespace GMS.Infrastructure.ViewModels.Guests;

public class GuestsListViewModel
{
    public GuestReservationRouteValues? GuestReservationRouteValues { get; set; }
    public Dictionary<string, bool>? VisibleSections { get; set; }
    public List<RoomRatesForEnquiry>? RoomRatesForEnquiryList { get; set; }
    public string? opt { get; set; }
    public string? PageSource { get; set; }
    public int? NoOfRooms { get; set; }
    public GMSFinalGuestDTO? Guest { get; set; }
    public List<GuestDocumentAttachmentsDTO>? GuestAttachments { get; set; }
    public List<GuaranteeCodeDTO>? PaymentMethods { get; set; }

    public List<GMSFinalGuestDTO>? Guests { get; set; }
    public SettlementDTO? SettlementDTO { get; set; }
    public MembersDetailsDTO? MemberDetail { get; set; }
    public List<GMSFinalGuestsWithChild>? GuestsWithChildren { get; set; }
    public List<MemberDetailsWithChild>? MemberDetailsWithChildren { get; set; }
    public GMSFinalGuestsWithChild? GuestsWithChild { get; set; }
    public GuestsGridViewParameters? GuestsGridViewParameters { get; set; }
    public List<GenderMasterDTO>? Genders { get; set; }
    public List<TblCountriesDTO>? Countries { get; set; }
    public List<TblStateDTO>? States { get; set; }
    public List<tblCityDTO>? Cities { get; set; }
    public List<ServicesDTO>? Services { get; set; }
    public List<RoomTypeDTO>? RoomTypes { get; set; }
    public List<CarTypeDTO>? CarTypes { get; set; }
    public List<BrandAwarenessDTO>? BrandAwarenesses { get; set; }
    public List<LeadSourceDTO>? LeadSources { get; set; }
    public List<SourcesDTO>? SourcesList { get; set; }
    public List<ChannelCodeDTO>? ChannelCodes { get; set; }
    public List<GuaranteeCodeDTO>? GuaranteeCodes { get; set; }
    public List<IFormFile>? PhotoAttachment { get; set; }
    public List<IFormFile>? IdProofAttachment { get; set; }
    public List<IFormFile>? PassportAttachment { get; set; }
    public List<IFormFile>? VisaAttachment { get; set; }
    public string? Source { get; set; }
    public string? BookStatus { get; set; }
    public RoomAllocationDTO? RoomAllocation { get; set; }
    public MembersDetailsDTO? MembersDetails { get; set; }
    public MemberDetailsWithChild? MembersWithAttributes { get; set; }
    public List<TaskMasterDTO>? ServicesForAddServices { get; set; }
    public List<TaskMasterDTO>? Tasks { get; set; }
    public List<PaymentDTO>? Payments { get; set; }
    public List<PaymentWithAttr>? PaymentsWithAttr { get; set; }
    public List<BillingDTO>? BillingDataList { get; set; }
    public bool? AccountSettled { get; set; }
    public CreditDebitNoteAccountDTO? CreditNoteAccount { get; set; }
}
