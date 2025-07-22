using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class SourcesRepository : DapperGenericRepository<Sources>, ISourcesRepository
{
    public SourcesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
