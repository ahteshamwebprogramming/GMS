using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class RoomAmenetiesRepository : DapperGenericRepository<RoomAmeneties>, IRoomAmenetiesRepository
    {
        public RoomAmenetiesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
