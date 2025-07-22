using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class CategoryMasterRepository : DapperGenericRepository<CategoryMaster>, ICategoryMasterRepository
{
    public CategoryMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}