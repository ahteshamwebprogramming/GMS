using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters
{
    public class TaskMasterViewModel
    {
        public TaskMasterDTO? TaskMaster { get; set; }
        public List<TaskMasterDTO>? TaskMasters { get; set; }
        public List<TaskMasterWithChild>? TaskMasterWithChildren { get; set; }
        public List<RoleMasterDTO>? Roles { get; set; }
    }
}
