namespace GMS.Infrastructure.ViewModels.Masters
{
    public class MasterScheduleWithChild
    {
        public int Id { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
        public TimeOnly Duration { get; set; }

        public int? TaskId { get; set; }
        public string? TaskName { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
