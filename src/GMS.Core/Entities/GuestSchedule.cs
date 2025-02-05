using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("GuestSchedule")]
public class GuestSchedule
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int GuestId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public TimeOnly Duration { get; set; }
    public int? TaskId { get; set; }
    public int? EmployeeId { get; set; }
    public int? ResourceId { get; set; }
}
