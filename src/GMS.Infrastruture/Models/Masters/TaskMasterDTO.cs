namespace GMS.Infrastructure.Models.Masters
{
    public class TaskMasterDTO
    {
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public int? Department { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
