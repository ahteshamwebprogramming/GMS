
using AutoMapper;
using GMS.Core.EHRMSEntities;
using GMS.Core.Entities;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.ReviewAndFeedback;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;

namespace GMS.Services.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<GMSFinalGuest, GMSFinalGuestDTO>().ReverseMap();
        CreateMap<EHRMSLogin, EHRMSLoginDTO>().ReverseMap();
        CreateMap<GenderMaster, GenderMasterDTO>().ReverseMap();
        CreateMap<tblCity, tblCityDTO>().ReverseMap();
        CreateMap<TblState, TblStateDTO>().ReverseMap();
        CreateMap<TblCountries, TblCountriesDTO>().ReverseMap();
        CreateMap<GMS.Core.Entities.Services, ServicesDTO>().ReverseMap();
        CreateMap<MstCategory, MstCategoryDTO>().ReverseMap();
        CreateMap<RoomType, RoomTypeDTO>().ReverseMap();
        CreateMap<CarType, CarTypeDTO>().ReverseMap();
        CreateMap<BrandAwareness, BrandAwarenessDTO>().ReverseMap();
        CreateMap<TBLLeadSource, LeadSourceDTO>().ReverseMap();
        CreateMap<ChannelCode, ChannelCodeDTO>().ReverseMap();
        CreateMap<GuaranteeCode, GuaranteeCodeDTO>().ReverseMap();
        CreateMap<MembersDetails, MembersDetailsDTO>().ReverseMap();
        CreateMap<RoomAllocation, RoomAllocationDTO>().ReverseMap();
        CreateMap<MasterSchedule, MasterScheduleDTO>().ReverseMap();
        CreateMap<GuestSchedule, GuestScheduleDTO>().ReverseMap();
        CreateMap<TaskMaster, TaskMasterDTO>().ReverseMap();
        CreateMap<ResourceMaster, ResourceMasterDTO>().ReverseMap();
        CreateMap<Rooms, RoomsDTO>().ReverseMap();
        CreateMap<RoomLock, RoomLockDTO>().ReverseMap();
        CreateMap<RoomsPictures, RoomsPicturesDTO>().ReverseMap();
        CreateMap<AmenetiesCategory, AmenetiesCategoryDTO>().ReverseMap();
        CreateMap<Amenities, AmenitiesDTO>().ReverseMap();
        CreateMap<RoomAmeneties, RoomAmenetiesDTO>().ReverseMap();
        CreateMap<TblCheckLists, TblCheckListsDTO>().ReverseMap();
        CreateMap<CategoryMaster, CategoryMasterDTO>().ReverseMap();
        CreateMap<GuestDocumentAttachments, GuestDocumentAttachmentsDTO>().ReverseMap();
        CreateMap<RoomChkList, RoomChkListDTO>().ReverseMap();
        CreateMap<Billing, BillingDTO>().ReverseMap();
        CreateMap<Payment, PaymentDTO>().ReverseMap();
        CreateMap<Settlement, SettlementDTO>().ReverseMap();
        CreateMap<FeedbackResults, FeedbackResultsDTO>().ReverseMap();
        CreateMap<FeedbackResultRatings, FeedbackResultRatingsDTO>().ReverseMap();
        CreateMap<PaymentMethod, PaymentMethodDTO>().ReverseMap();
        CreateMap<Feedback, FeedbackDTO>().ReverseMap();

        CreateMap<AuditedRevenue, AuditedRevenueDTO>().ReverseMap();
        CreateMap<GuestReservation, GuestReservationDTO>().ReverseMap();
        CreateMap<Sources, SourcesDTO>().ReverseMap();
        CreateMap<CreditDebitNoteAccount, CreditDebitNoteAccountDTO>().ReverseMap();
        CreateMap<GuestLedger, GuestLedgerDTO>().ReverseMap();
    }
}
