using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class TaskMasterRepository : DapperGenericRepository<TaskMaster>, ITaskMasterRepository
    {
        public TaskMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
