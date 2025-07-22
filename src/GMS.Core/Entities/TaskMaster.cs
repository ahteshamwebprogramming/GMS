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
    public int? CategoryId { get; set; }
    public TimeOnly? Duration { get; set; }
    public decimal? Rate { get; set; }
    public bool? DoctorAdviceRequired { get; set; }
    public string? Remarks { get; set; }
}
