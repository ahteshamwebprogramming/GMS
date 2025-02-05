using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class RoomAllocationRepository : DapperGenericRepository<RoomAllocation>, IRoomAllocationRepository
{
    public RoomAllocationRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
