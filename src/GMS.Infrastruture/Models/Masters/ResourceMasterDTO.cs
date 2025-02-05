namespace GMS.Infrastructure.Models.Masters;

public class ResourceMasterDTO
{
    public int Id { get; set; }
    public string? ResourceName { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
