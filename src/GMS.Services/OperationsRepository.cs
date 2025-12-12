using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class OperationsRepository : DapperGenericRepository<Operations>, IOperationsRepository
{
    public OperationsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
    }
}

