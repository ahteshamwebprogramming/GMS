using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class ServicesRepository : DapperGenericRepository<GMS.Core.Entities.Services>, IServicesRepository
{
    public ServicesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
