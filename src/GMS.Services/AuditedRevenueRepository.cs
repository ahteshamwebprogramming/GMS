using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class AuditedRevenueRepository : DapperGenericRepository<AuditedRevenue>, IAuditedRevenueRepository
{
    public AuditedRevenueRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}