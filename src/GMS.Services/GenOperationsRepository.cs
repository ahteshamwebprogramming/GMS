using GMS.Core.Entities;
using GMS.Services.DBContext;
using GMS.Core.Repository;

namespace GMS.Services
{
    public class GenOperationsRepository : DapperGenericRepository<GenOperations>, IGenOperationsRepository
    {
        public GenOperationsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {
        }
    }
}
