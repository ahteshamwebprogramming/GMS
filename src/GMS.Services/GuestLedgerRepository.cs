using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GuestLedgerRepository : DapperGenericRepository<GuestLedger>, IGuestLedgerRepository
{
    public GuestLedgerRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
