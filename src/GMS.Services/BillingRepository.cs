using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class BillingRepository : DapperGenericRepository<Billing>, IBillingRepository
{
    public BillingRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
