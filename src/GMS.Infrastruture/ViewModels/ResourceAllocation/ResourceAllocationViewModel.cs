using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.ResourceAllocation;

public class ResourceAllocationViewModel
{
    public List<GuestScheduleWithAttributes>? ScheduleWithAttributeList { get; set; }
    public List<TaskMasterDTO>? Tasks { get; set; }
}
