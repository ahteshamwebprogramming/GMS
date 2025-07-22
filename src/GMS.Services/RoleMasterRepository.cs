using GMS.Core.EHRMSEntities;
using GMS.Core.EHRMSRepository;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class RoleMasterRepository : DapperGenericEHRMSRepository<RoleMaster>, IRoleMasterRepository
{
    public RoleMasterRepository(DapperEHRMSDBContext dapperEHRMDBContext) : base(dapperEHRMDBContext)
    {

    }
}
