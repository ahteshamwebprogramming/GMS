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
    }

}
