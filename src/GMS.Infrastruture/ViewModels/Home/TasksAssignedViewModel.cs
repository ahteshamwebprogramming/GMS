using System;
using System.Collections.Generic;
using System.Linq;
using GMS.Infrastructure.ViewModels.Rooms;

namespace GMS.Infrastructure.ViewModels.Home;

public class TasksAssignedViewModel
{
    public DateTime WorkDate { get; set; }
    public int WorkerId { get; set; }
    public string? WorkerName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public int LoggedInWorkerId { get; set; }
    public string? LoggedInWorkerName { get; set; }
    public List<HousekeepingAssignmentRow> Assignments { get; set; } = new();
    public List<EmployeeOption> TeamMembers { get; set; } = new();

    public bool IsViewingOtherEmployee => WorkerId != LoggedInWorkerId && LoggedInWorkerId > 0;
    public int TotalAssigned => Assignments?.Count ?? 0;
    public int PendingCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Pending) ?? 0;
    public int InProgressCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.InProgress) ?? 0;
    public int CompletedCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Completed) ?? 0;
    public int InspectionCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Inspection) ?? 0;
    public bool HasAssignments => TotalAssigned > 0;
}

public class EmployeeOption
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

