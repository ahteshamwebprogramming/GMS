using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters;

public class MasterScheduleViewModel
{
    public MasterScheduleDTO? MasterSchedule { get; set; }
    public List<MasterScheduleDTO>? MasterSchedules { get; set; }
    public List<MasterScheduleWithChild>? MasterScheduleWithChildren { get; set; }
    public List<TaskMasterDTO>? TaskList { get; set; }
}
