using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class BrandAwarenessRepository : DapperGenericRepository<BrandAwareness>, IBrandAwarenessRepository
{
    public BrandAwarenessRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
