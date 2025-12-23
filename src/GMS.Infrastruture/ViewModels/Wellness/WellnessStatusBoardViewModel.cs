namespace GMS.Infrastructure.ViewModels.Wellness;

public class WellnessStatusBoardViewModel
{
    public List<WellnessScheduleRow> Schedules { get; set; } = new();
    public DateTime BoardDate { get; set; } = DateTime.Today;
}

public class WellnessScheduleRow
{
    public int ScheduleId { get; set; }
    public int? ExecutionId { get; set; }
    
    // Therapist Info
    public string? TherapistName { get; set; }
    public int? TherapistId { get; set; }
    
    // Treatment Info
    public string? TreatmentName { get; set; }
    public int? TaskId { get; set; }
    
    // Schedule Info
    public DateTime ScheduledDateTime { get; set; }
    public DateTime? ScheduledEndDateTime { get; set; }
    public TimeSpan? Duration { get; set; }
    
    // Room Info
    public string? TreatmentRoom { get; set; }
    public int? ResourceId { get; set; }
    
    // Guest Info
    public string? GuestName { get; set; }
    public int? GuestId { get; set; }
    public string? RoomNo { get; set; }
    
    // Execution Status
    public WellnessExecutionStatus Status { get; set; } = WellnessExecutionStatus.Scheduled;
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int? DelayMinutes { get; set; }
    
    // Remarks
    public string? Remarks { get; set; }
    public bool? IssueReported { get; set; }
    public string? IssueNotes { get; set; }
    
    // Computed properties
    public bool IsOverdue => Status != WellnessExecutionStatus.Completed 
                            && Status != WellnessExecutionStatus.Cancelled 
                            && DateTime.Now > ScheduledDateTime;
    
    public string StatusText => Status switch
    {
        WellnessExecutionStatus.Scheduled => "Scheduled",
        WellnessExecutionStatus.Assigned => "Assigned",
        WellnessExecutionStatus.InProgress => "In Progress",
        WellnessExecutionStatus.Completed => "Completed",
        WellnessExecutionStatus.Pending => "Pending",
        WellnessExecutionStatus.Cancelled => "Cancelled",
        _ => "Unknown"
    };
    
    public string StatusClass => Status switch
    {
        WellnessExecutionStatus.Scheduled => "scheduled",
        WellnessExecutionStatus.Assigned => "assigned",
        WellnessExecutionStatus.InProgress => "in-progress",
        WellnessExecutionStatus.Completed => "completed",
        WellnessExecutionStatus.Pending => "pending",
        WellnessExecutionStatus.Cancelled => "cancelled",
        _ => "scheduled"
    };
}

public enum WellnessExecutionStatus
{
    Scheduled = 0,
    Assigned = 1,
    Pending = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}







