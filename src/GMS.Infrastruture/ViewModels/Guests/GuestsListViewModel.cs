using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Guests;

public class GuestsListViewModel
{
    public GMSFinalGuestDTO? Guest { get; set; }
    public List<GMSFinalGuestDTO>? Guests { get; set; }
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
    public List<ChannelCodeDTO>? ChannelCodes { get; set; }
    public List<GuaranteeCodeDTO>? GuaranteeCodes { get; set; }
    public List<IFormFile>? PhotoAttachment { get; set; }
    public List<IFormFile>? IdProofAttachment { get; set; }
    public List<IFormFile>? PassportAttachment { get; set; }
    public string? Source { get; set; }
    public string? BookStatus { get; set; }
    public RoomAllocationDTO? RoomAllocation { get; set; }
}
