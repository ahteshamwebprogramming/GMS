using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class LeadSourceRepository : DapperGenericRepository<TBLLeadSource>, ILeadSourceRepository
{
    public LeadSourceRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
