using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class RoomChkListRepository : DapperGenericRepository<RoomChkList>, IRoomChkListRepository
    {
        public RoomChkListRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
