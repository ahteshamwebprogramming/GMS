using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class ChannelCodeRepository : DapperGenericRepository<ChannelCode>, IChannelCodeRepository
{
    public ChannelCodeRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
