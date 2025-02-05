namespace GMS.Infrastructure.ViewModels.Masters
{
    public class ResourceMasterWithChild
    {
        public int Id { get; set; }
        public string? ResourceName { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? RoleName { get; set; }
    }
}
