using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class MasterScheduleRepository : DapperGenericRepository<MasterSchedule>, IMasterScheduleRepository
{
    public MasterScheduleRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
