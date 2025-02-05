using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class RoomTypeRepository : DapperGenericRepository<RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}