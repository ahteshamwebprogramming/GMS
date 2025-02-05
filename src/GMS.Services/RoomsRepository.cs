using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class RoomsRepository : DapperGenericRepository<Rooms>, IRoomsRepository
    {
        public RoomsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
