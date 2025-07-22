namespace GMS.Infrastructure.ViewModels.ResourceAllocation;

public class GuestScheduleWithAttributes
{
    public int Id { get; set; }
    public int GuestId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int? TaskId { get; set; }
    public int? EmployeeId1 { get; set; }
    public int? EmployeeId2 { get; set; }
    public int? SessionId { get; set; }
    public int? ResourceId { get; set; }
    public string? TaskName { get; set; }
    public string? ResourceName { get; set; }
    public string? Therapist1Name { get; set; }
    public string? Therapist2Name { get; set; }
    public string? Therapist3Name { get; set; }
    public string? GuestName { get; set; }
    public string? RoomNo { get; set; }
    public bool? InPackage { get; set; }
    public string? Remarks { get; set; }
}
