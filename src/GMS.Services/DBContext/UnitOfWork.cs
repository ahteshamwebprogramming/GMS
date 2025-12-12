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
        RoleMaster = new RoleMasterRepository(_dapperEHRMSDBContext);
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
        CategoryMaster = new CategoryMasterRepository(_dapperDBContext);
        GuestDocumentAttachments = new GuestDocumentAttachmentsRepository(_dapperDBContext);
        RoomChkList = new RoomChkListRepository(_dapperDBContext);
        Billing = new BillingRepository(_dapperDBContext);
        Payment = new PaymentRepository(_dapperDBContext);
        Settlement = new SettlementRepository(_dapperDBContext);
        FeedbackResults = new FeedbackResultsRepository(_dapperDBContext);
        FeedbackResultRatings = new FeedbackResultRatingsRepository(_dapperDBContext);
        Rates = new RatesRepository(_dapperDBContext);
        PaymentMethod = new PaymentMethodRepository(_dapperDBContext);
        Feedback = new FeedbackRepository(_dapperDBContext);
        AuditedRevenue = new AuditedRevenueRepository(_dapperDBContext);
        GuestReservation = new GuestReservationRepository(_dapperDBContext);
        GuestCancellationVerification = new GuestCancellationVerificationRepository(_dapperDBContext);
        Sources = new SourcesRepository(_dapperDBContext);
        CreditDebitNoteAccount = new CreditDebitNoteAccountRepository(_dapperDBContext);
        GuestLedger = new GuestLedgerRepository(_dapperDBContext);
        MenuList = new MenuListRepository(_dapperDBContext);
        RoleMenuMapping = new RoleMenuMappingRepository(_dapperDBContext);
        Operations = new OperationsRepository(_dapperDBContext);
    }


    public IGMSFinalGuestRepository GMSFinalGuest { get; private set; }
    public IEHRMSLoginRepository EHRMSLogin { get; private set; }
    public IRoleMasterRepository RoleMaster { get; private set; }
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
    public ICategoryMasterRepository CategoryMaster { get; private set; }
    public IGuestDocumentAttachmentsRepository GuestDocumentAttachments { get; private set; }
    public IRoomChkListRepository RoomChkList { get; private set; }
    public IBillingRepository Billing { get; private set; }
    public IPaymentRepository Payment { get; private set; }
    public ISettlementRepository Settlement { get; private set; }
    public IFeedbackResultsRepository FeedbackResults { get; private set; }
    public IFeedbackResultRatingsRepository FeedbackResultRatings { get; private set; }
    public IRatesRepository Rates { get; private set; }
    public IPaymentMethodRepository PaymentMethod { get; private set; }
    public IFeedbackRepository Feedback { get; private set; }

    public IAuditedRevenueRepository AuditedRevenue { get; private set; }
    public IGuestReservationRepository GuestReservation { get; private set; }
    public IGuestCancellationVerificationRepository GuestCancellationVerification { get; private set; }
    public ISourcesRepository Sources { get; private set; }
    public ICreditDebitNoteAccountRepository CreditDebitNoteAccount { get; private set; }
    public IGuestLedgerRepository GuestLedger { get; private set; }
    public IMenuListRepository MenuList { get; private set; }
    public IRoleMenuMappingRepository RoleMenuMapping { get; private set; }
    public IOperationsRepository Operations { get; private set; }

    public void BeginTransaction()
    {
        _dapperDBContext.BeginTransaction();
    }

    public void Commit()
    {
        _dapperDBContext.Commit();
    }

    public void Rollback()
    {
        _dapperDBContext.Rollback();
    }

    public void Dispose()
    {
        _dapperDBContext.Dispose();
    }
}
