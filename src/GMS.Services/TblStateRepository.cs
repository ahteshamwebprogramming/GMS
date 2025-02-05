using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class TblStateRepository : DapperGenericRepository<TblState>, ITblStateRepository
{
    public TblStateRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
