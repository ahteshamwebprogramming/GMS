using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GenderMasterRepository : DapperGenericRepository<GenderMaster>, IGenderMasterRepository
{
    public GenderMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
