using GMS.Core.EHRMSEntities;
using GMS.Core.EHRMSRepository;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class EHRMSLoginRepository : DapperGenericEHRMSRepository<EHRMSLogin>, IEHRMSLoginRepository
{
    public EHRMSLoginRepository(DapperEHRMSDBContext dapperEHRMDBContext) : base(dapperEHRMDBContext)
    {

    }
}
