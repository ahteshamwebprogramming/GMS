using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GuestScheduleRepository : DapperGenericRepository<GuestSchedule>, IGuestScheduleRepository
{
    public GuestScheduleRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
