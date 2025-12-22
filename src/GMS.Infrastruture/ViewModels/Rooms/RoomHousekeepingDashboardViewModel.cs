using System;
using System.Collections.Generic;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomHousekeepingDashboardViewModel
    {
        public DateTime WorkDate { get; set; }
        public int RoomsCompleted { get; set; }
        public int TotalRooms { get; set; }
        public int RoomsPending { get; set; }
        public List<string> Alerts { get; set; } = new();
        public List<HousekeepingWorkerSummary> Team { get; set; } = new();
        public List<HousekeepingRoomCard> UnassignedRooms { get; set; } = new();
        public List<HousekeepingAssignmentRow> AssignmentMatrix { get; set; } = new();
        public List<string> RoomTypes { get; set; } = new();
        public List<HousekeepingDepartmentOption>? Departments { get; set; }
    }

    public class HousekeepingWorkerSummary
    {
        public int WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public int AssignedRoomCount { get; set; }
        public int PendingRoomCount { get; set; }
    }

    public class HousekeepingRoomCard
    {
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public string? AvailabilityStatus { get; set; }
        public string? TidyStatus { get; set; }
    }

    public class HousekeepingAssignmentRow
    {
        public int ScheduleId { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public string? OccupancyStatus { get; set; }
        public bool IsAssigned { get; set; }
        public string? AssignedTo { get; set; }
        public HousekeepingAssignmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public bool IssueReported { get; set; }
        public string? IssueNotes { get; set; }
        public string? GuestName { get; set; }
        public string? TreatmentRoom { get; set; }
        public string? OtherAssignees { get; set; }
    }

    public enum HousekeepingAssignmentStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Inspection = 3
    }

    public class HousekeepingDepartmentOption
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class RoomChecklistReviewViewModel
    {
        public int RoomId { get; set; }
        public List<int> ChecklistItemIds { get; set; } = new();
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string? Reason { get; set; }
        public string? Comments { get; set; }
    }
}

