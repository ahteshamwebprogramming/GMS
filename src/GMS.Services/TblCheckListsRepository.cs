using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class TblCheckListsRepository : DapperGenericRepository<TblCheckLists>, ITblCheckListsRepository
{
    public TblCheckListsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}