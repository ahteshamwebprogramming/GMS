using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class SettlementRepository : DapperGenericRepository<Settlement>, ISettlementRepository
{
    public SettlementRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
