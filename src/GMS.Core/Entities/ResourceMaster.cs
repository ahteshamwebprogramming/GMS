using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("ResourceMaster")]

public class ResourceMaster
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? ResourceName { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
