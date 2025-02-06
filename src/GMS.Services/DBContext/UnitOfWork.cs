using AutoMapper;
using GMS.Core.EHRMSEntities;
using GMS.Core.EHRMSRepository;
using GMS.Core.Entities;
using GMS.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace GMS.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperDBContext _dapperDBContext;
    private readonly DapperEHRMSDBContext _dapperEHRMSDBContext;
    public UnitOfWork(DapperDBContext dapperDBContext, DapperEHRMSDBContext dapperEHRMSDBContext, IMapper mapper)
    {
        _dapperDBContext = dapperDBContext;
        _dapperEHRMSDBContext = dapperEHRMSDBContext;
        GMSFinalGuest = new GMSFinalGuestRepository(_dapperDBContext);
        EHRMSLogin = new EHRMSLoginRepository(_dapperEHRMSDBContext);
        GenderMaster = new GenderMasterRepository(_dapperDBContext);
        tblCity = new tblCityRepository(_dapperDBContext);
        TblState = new TblStateRepository(_dapperDBContext);
        TblCountries = new TblCountriesRepository(_dapperDBContext);
        MembersDetails = new MembersDetailsRepository(_dapperDBContext);
        Services = new ServicesRepository(_dapperDBContext);
        MstCategory = new MstCategoryRepository(_dapperDBContext);
        RoomType = new RoomTypeRepository(_dapperDBContext);
        CarType = new CarTypeRepository(_dapperDBContext);
        BrandAwareness = new BrandAwarenessRepository(_dapperDBContext);
        LeadSource = new LeadSourceRepository(_dapperDBContext);
        ChannelCode = new ChannelCodeRepository(_dapperDBContext);
        GuaranteeCode = new GuaranteeCodeRepository(_dapperDBContext);
        RoomAllocation = new RoomAllocationRepository(_dapperDBContext);
        MasterSchedule = new MasterScheduleRepository(_dapperDBContext);
        GuestSchedule = new GuestScheduleRepository(_dapperDBContext);
        TaskMaster = new TaskMasterRepository(_dapperDBContext);
        ResourceMaster = new ResourceMasterRepository(_dapperDBContext);
        GenOperations = new GenOperationsRepository(_dapperDBContext);
        Rooms = new RoomsRepository(_dapperDBContext);
        RoomLock = new RoomLockRepository(_dapperDBContext);
        RoomsPictures = new RoomsPicturesRepository(_dapperDBContext);
        AmenetiesCategory = new AmenetiesCategoryRepository(_dapperDBContext);
        Amenities = new AmenitiesRepository(_dapperDBContext);
        RoomAmeneties = new RoomAmenetiesRepository(_dapperDBContext);
        TblCheckLists = new TblCheckListsRepository(_dapperDBContext);
    }


    public IGMSFinalGuestRepository GMSFinalGuest { get; private set; }
    public IEHRMSLoginRepository EHRMSLogin { get; private set; }
    public IGenderMasterRepository GenderMaster { get; private set; }
    public ItblCityRepository tblCity { get; private set; }
    public ITblStateRepository TblState { get; private set; }
    public ITblCountriesRepository TblCountries { get; private set; }
    public IMembersDetailsRepository MembersDetails { get; private set; }
    public IServicesRepository Services { get; private set; }
    public IMstCategoryRepository MstCategory { get; private set; }
    public IRoomTypeRepository RoomType { get; private set; }
    public ICarTypeRepository CarType { get; private set; }
    public IBrandAwarenessRepository BrandAwareness { get; private set; }
    public ILeadSourceRepository LeadSource { get; private set; }
    public IChannelCodeRepository ChannelCode { get; private set; }
    public IGuaranteeCodeRepository GuaranteeCode { get; private set; }
    public IRoomAllocationRepository RoomAllocation { get; private set; }
    public IMasterScheduleRepository MasterSchedule { get; private set; }
    public IGuestScheduleRepository GuestSchedule { get; private set; }
    public ITaskMasterRepository TaskMaster { get; private set; }
    public IResourceMasterRepository ResourceMaster { get; private set; }
    public IGenOperationsRepository GenOperations { get; private set; }
    public IRoomsRepository Rooms { get; private set; }
    public IRoomLockRepository RoomLock { get; private set; }
    public IRoomsPicturesRepository RoomsPictures { get; private set; }
    public IAmenetiesCategoryRepository AmenetiesCategory { get; private set; }
    public IAmenitiesRepository Amenities { get; private set; }
    public IRoomAmenetiesRepository RoomAmeneties { get; private set; }
    public ITblCheckListsRepository TblCheckLists { get; private set; }

}
