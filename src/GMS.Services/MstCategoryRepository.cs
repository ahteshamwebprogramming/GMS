using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class MstCategoryRepository : DapperGenericRepository<MstCategory>, IMstCategoryRepository
{
    public MstCategoryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
