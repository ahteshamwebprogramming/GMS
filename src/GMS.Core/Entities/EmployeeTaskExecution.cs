using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeTaskExecution")]
public class EmployeeTaskExecution
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int GuestScheduledTaskId { get; set; }  // FK to GuestSchedule.Id
    public int EmployeeId { get; set; }            // Worker ID

    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }

    public string? ExecutionStatus { get; set; }   // Pending / Started / Completed / Missed / Issue
    public int? DelayMinutes { get; set; }         // Calculated when started late

    public string? EscalationLevel { get; set; }   // None / Warning / Critical
    public bool? IssueReportedBit { get; set; }
    public string? IssueNotes { get; set; }

    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}








