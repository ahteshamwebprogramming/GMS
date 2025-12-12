using System;
using System.Collections.Generic;
using System.Linq;

namespace GMS.Infrastructure.ViewModels.Rooms;

public class HouseKeeperDashboardViewModel
{
    public DateTime WorkDate { get; set; }
    public int WorkerId { get; set; }
    public string? WorkerName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public List<HousekeepingAssignmentRow> Assignments { get; set; } = new();

    public int TotalAssigned => Assignments?.Count ?? 0;
    public int PendingCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Pending) ?? 0;
    public int InProgressCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.InProgress) ?? 0;
    public int CompletedCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Completed) ?? 0;
    public int InspectionCount => Assignments?.Count(a => a.Status == HousekeepingAssignmentStatus.Inspection) ?? 0;
    public bool HasAssignments => TotalAssigned > 0;
}

