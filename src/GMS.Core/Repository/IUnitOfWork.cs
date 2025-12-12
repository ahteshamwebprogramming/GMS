using GMS.Core.EHRMSRepository;

namespace GMS.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public IGMSFinalGuestRepository GMSFinalGuest { get; }
    public IEHRMSLoginRepository EHRMSLogin { get; }
    public IRoleMasterRepository RoleMaster { get; }
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
    public ICategoryMasterRepository CategoryMaster { get; }
    public IGuestDocumentAttachmentsRepository GuestDocumentAttachments { get; }
    public IRoomChkListRepository RoomChkList { get; }
    public IBillingRepository Billing { get; }
    public IPaymentRepository Payment { get; }
    public ISettlementRepository Settlement { get; }
    public IFeedbackResultsRepository FeedbackResults { get; }
    public IFeedbackResultRatingsRepository FeedbackResultRatings { get; }
    public IRatesRepository Rates { get; }
    public IPaymentMethodRepository PaymentMethod { get; }
    public IFeedbackRepository Feedback { get; }
    public IAuditedRevenueRepository AuditedRevenue { get; }
    public IGuestReservationRepository GuestReservation { get; }
    public IGuestCancellationVerificationRepository GuestCancellationVerification { get; }
    public ISourcesRepository Sources { get; }
    public ICreditDebitNoteAccountRepository CreditDebitNoteAccount { get; }
    public IGuestLedgerRepository GuestLedger { get; }
    public IMenuListRepository MenuList { get; }
    public IRoleMenuMappingRepository RoleMenuMapping { get; }
    public IOperationsRepository Operations { get; }

    void BeginTransaction();
    void Commit();
    void Rollback();

}
