using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class MembersDetailsRepository : DapperGenericRepository<MembersDetails>, IMembersDetailsRepository
{
    public MembersDetailsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
