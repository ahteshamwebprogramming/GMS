using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class tblCityRepository : DapperGenericRepository<tblCity>, ItblCityRepository
{
    public tblCityRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
