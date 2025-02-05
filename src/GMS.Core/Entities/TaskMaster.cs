using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("TaskMaster")]

public class TaskMaster
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? TaskName { get; set; }
    public int? Department { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }    
}
