using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Guests
{
    public class GuestScheduleViewModel
    {
        public GuestScheduleDTO? GuestSchedule { get; set; }
        public List<TaskMasterDTO>? Tasks { get; set; }
    }
}
