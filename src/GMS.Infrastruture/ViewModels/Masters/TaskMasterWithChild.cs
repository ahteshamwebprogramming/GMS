namespace GMS.Infrastructure.ViewModels.Masters
{
    public class TaskMasterWithChild
    {
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public int? Department { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? RoleName { get; set; }
        public bool? Readonly { get; set; }

        public int? CategoryId { get; set; }
        public TimeOnly? Duration { get; set; }
        public decimal? Rate { get; set; }
        public bool? DoctorAdviceRequired { get; set; }
        public string? Remarks { get; set; }
        public string? Category { get; set; }
    }

}
