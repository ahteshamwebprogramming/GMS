
using AutoMapper;
using GMS.Core.EHRMSEntities;
using GMS.Core.Entities;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
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
    }
}
