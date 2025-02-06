using GMS.Core.EHRMSRepository;

namespace GMS.Core.Repository;

public interface IUnitOfWork
{
    public IGMSFinalGuestRepository GMSFinalGuest { get; }
    public IEHRMSLoginRepository EHRMSLogin { get; }
    public IGenderMasterRepository GenderMaster { get; }
    public ItblCityRepository tblCity { get; }
    public ITblStateRepository TblState { get; }
    public ITblCountriesRepository TblCountries { get; }
    public IMembersDetailsRepository MembersDetails { get; }
    public IServicesRepository Services { get; }
    public IMstCategoryRepository MstCategory { get; }
    public IRoomTypeRepository RoomType { get; }
    public ICarTypeRepository CarType { get; }
    public IBrandAwarenessRepository BrandAwareness { get; }
    public ILeadSourceRepository LeadSource { get; }
    public IChannelCodeRepository ChannelCode { get; }
    public IGuaranteeCodeRepository GuaranteeCode { get; }
    public IRoomAllocationRepository RoomAllocation { get; }
    public IMasterScheduleRepository MasterSchedule { get; }
    public IGuestScheduleRepository GuestSchedule { get; }
    public ITaskMasterRepository TaskMaster { get; }
    public IResourceMasterRepository ResourceMaster { get; }
    public IGenOperationsRepository GenOperations { get; }
    public IRoomsRepository Rooms { get; }
    public IRoomLockRepository RoomLock { get; }
    public IRoomsPicturesRepository RoomsPictures { get; }
    public IAmenetiesCategoryRepository AmenetiesCategory { get; }
    public IAmenitiesRepository Amenities { get; }
    public IRoomAmenetiesRepository RoomAmeneties { get; }
    public ITblCheckListsRepository TblCheckLists { get; }

}
