using Dapper;
using GMS.Core.Entities;
using GMS.Endpoints.Rooms;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;
using GMS.Services.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;
using System.Linq;

namespace GMS.WebUI.Services.Rooms;

public class RoomHousekeepingDashboardService
{
    private readonly RoomLockingAPIController _roomLockingAPIController;
    private readonly DapperEHRMSDBContext _ehrmsDbContext;
    private readonly DapperDBContext _dbContext;
    private readonly ILogger<RoomHousekeepingDashboardService> _logger;

    public RoomHousekeepingDashboardService(RoomLockingAPIController roomLockingAPIController,
        ILogger<RoomHousekeepingDashboardService> logger,
        DapperEHRMSDBContext ehrmsDbContext,
        DapperDBContext dbContext)
    {
        _roomLockingAPIController = roomLockingAPIController;
        _logger = logger;
        _ehrmsDbContext = ehrmsDbContext;
        _dbContext = dbContext;
    }

    public async Task<RoomHousekeepingDashboardViewModel> GetDashboardAsync(DateTime workDate)
    {
        var rooms = await FetchRoomsAsync();
        var roster = await GetRosterAsync();
        var rosterLookup = roster.ToDictionary(w => w.WorkerId);
        var assignments = await BuildAssignmentLookupAsync(workDate, rosterLookup);
        var departments = await GetDepartmentsAsync();
        var occupancyMap = await BuildRoomOccupancyMapAsync(workDate);

        var assignmentRows = rooms.Select(room =>
        {
            var assignment = assignments.TryGetValue(room.Id, out var state) ? state : null;
            var status = DetermineStatus(room, assignment);
            var assignedTo = assignment?.WorkerName ?? room.AttendedByName ?? "Unassigned";
            var occupancyStatus = ResolveOccupancyLabel(room, occupancyMap);
            return new HousekeepingAssignmentRow
            {
                RoomId = room.Id,
                RoomNumber = room.Rnumber,
                RoomType = room.Rtype,
                OccupancyStatus = occupancyStatus,
                IsAssigned = assignment != null,
                AssignedTo = assignment != null ? assignedTo : null,
                Status = status,
                Notes = assignment?.Notes ?? room.Comments
            };
        }).ToList();

        foreach (var worker in roster)
        {
            worker.AssignedRoomCount = assignments.Values.Count(a => a.WorkerId == worker.WorkerId);
            worker.PendingRoomCount = assignmentRows.Count(row => row.Status == HousekeepingAssignmentStatus.Pending &&
                                                                  row.AssignedTo == worker.WorkerName);
        }

        var unassignedRooms = rooms
            .Where(r => !assignments.ContainsKey(r.Id))
            .Select(r =>
            {
                var occupancyStatus = ResolveOccupancyLabel(r, occupancyMap);
                return new HousekeepingRoomCard
            {
                RoomId = r.Id,
                RoomNumber = r.Rnumber,
                RoomType = r.Rtype,
                AvailabilityStatus = occupancyStatus,
                TidyStatus = r.CleanStatus ? "Ready" : "Untidy"
            };
            }).ToList();

        var completed = assignmentRows.Count(r => r.Status == HousekeepingAssignmentStatus.Completed);

        return new RoomHousekeepingDashboardViewModel
        {
            WorkDate = workDate,
            RoomsCompleted = completed,
            TotalRooms = rooms.Count,
            RoomsPending = rooms.Count - completed,
            Team = roster,
            UnassignedRooms = unassignedRooms,
            AssignmentMatrix = assignmentRows,
            Departments = departments
        };
    }

    public async Task<List<HousekeepingRoomCard>> GetUnassignedRoomsAsync(DateTime workDate)
    {
        var dashboard = await GetDashboardAsync(workDate);
        return dashboard.UnassignedRooms;
    }

    public async Task<List<HousekeepingAssignmentRow>> GetWorkerAssignmentsAsync(DateTime workDate, int workerId)
    {
        var rooms = await FetchRoomsAsync();
        var assignments = await FetchAssignmentsFromDbAsync(workDate);
        var occupancyMap = await BuildRoomOccupancyMapAsync(workDate);
        var roster = await GetRosterAsync();
        var workerLookup = roster.ToDictionary(w => w.WorkerId);
        var rows = new List<HousekeepingAssignmentRow>();

        foreach (var assignment in assignments.Where(a => (int)a.WorkerId == workerId))
        {
            var room = rooms.FirstOrDefault(r => r.Id == assignment.RoomId);
            if (room == null)
            {
                continue;
            }
            var worker = workerLookup.TryGetValue(workerId, out var summary) ? summary : null;
            rows.Add(new HousekeepingAssignmentRow
            {
                RoomId = room.Id,
                RoomNumber = room.Rnumber,
                RoomType = room.Rtype,
                OccupancyStatus = ResolveOccupancyLabel(room, occupancyMap),
                IsAssigned = true,
                AssignedTo = worker?.WorkerName ?? $"Worker #{workerId}",
                Status = (HousekeepingAssignmentStatus)assignment.Status,
                Notes = assignment.Notes
            });
        }
        return rows;
    }

    public async Task<List<TblCheckListsDTO>> GetRoomChecklistAsync()
    {
        try
        {
            var res = await _roomLockingAPIController.GetRoomCleanCheckList();
            if (res is ObjectResult objectResult && objectResult.Value is List<TblCheckListsDTO> dto)
            {
                return dto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load room cleaning checklist.");
        }
        return new List<TblCheckListsDTO>();
    }

    public async Task<RoomChecklistReviewViewModel?> GetRoomChecklistReviewAsync(int roomId)
    {
        if (roomId <= 0)
        {
            return null;
        }

        const string sql = @"SELECT TOP 1 
                                    rcl.Id,
                                    rcl.RID AS RoomId,
                                    rcl.RChkLstID,
                                    rcl.ChkDate,
                                    rcl.CheckedBy,
                                    rcl.Reason,
                                    rcl.Comments,
                                    wm.WorkerName AS ReviewedByName
                             FROM RoomChkList rcl WITH (NOLOCK)
                             LEFT JOIN EHRMS.dbo.WorkerMaster wm WITH (NOLOCK) ON wm.WorkerID = rcl.CheckedBy
                             WHERE rcl.RID = @RoomId
                             ORDER BY rcl.ChkDate DESC";

        try
        {
            using var connection = _dbContext.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<RoomChecklistReviewRow>(sql, new { RoomId = roomId });
            if (row == null)
            {
                return null;
            }

            return new RoomChecklistReviewViewModel
            {
                RoomId = roomId,
                ChecklistItemIds = ParseChecklistIds(row.RChkLstID),
                ReviewedBy = !string.IsNullOrWhiteSpace(row.ReviewedByName)
                    ? row.ReviewedByName
                    : row.CheckedBy.HasValue ? $"Worker #{row.CheckedBy.Value}" : null,
                ReviewedOn = row.ChkDate,
                Reason = row.Reason,
                Comments = row.Comments
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load checklist review for room {RoomId}", roomId);
            return null;
        }
    }

    public async Task MarkRoomCleanAsync(HousekeeperCleanRequest request, int workerId)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        if (request.RoomId <= 0)
        {
            throw new InvalidOperationException("Room information is missing.");
        }
        var workDate = request.WorkDate == default ? DateTime.Today : request.WorkDate.Date;

        await SaveRoomChecklistAsync(request, workerId);
        await UpdateAssignmentCompletionAsync(request.RoomId, workDate, workerId, request.Comments);
    }

    public async Task<HouseKeeperDashboardViewModel> GetWorkerDashboardAsync(DateTime workDate, int workerId)
    {
        var assignments = await GetWorkerAssignmentsAsync(workDate, workerId);
        var roster = await GetRosterAsync();
        var worker = roster.FirstOrDefault(w => w.WorkerId == workerId);

        return new HouseKeeperDashboardViewModel
        {
            WorkDate = workDate,
            WorkerId = workerId,
            WorkerName = worker?.WorkerName ?? $"Worker #{workerId}",
            EmployeeCode = worker?.EmployeeCode,
            Department = worker?.Department,
            Assignments = assignments
        };
    }

    public async Task AssignRoomsAsync(AssignRoomsRequest request)
    {
        var rooms = await FetchRoomsAsync();
        var roster = await GetRosterAsync();
        var worker = roster.FirstOrDefault(w => w.WorkerId == request.WorkerId);
        if (worker == null)
        {
            throw new InvalidOperationException("Worker not found for assignment.");
        }

        var existingAssignments = await FetchAssignmentsFromDbAsync(request.WorkDate);
        var assignedRoomIds = new HashSet<int>(existingAssignments.Select(a => a.RoomId));

        var newAssignments = new List<RoomHousekeepingAssignment>();
        foreach (var roomId in request.RoomIds ?? Enumerable.Empty<int>())
        {
            var room = rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                throw new InvalidOperationException($"Room {roomId} not found.");
            }
            if (assignedRoomIds.Contains(roomId))
            {
                continue;
            }

            newAssignments.Add(new RoomHousekeepingAssignment
            {
                RoomId = roomId,
                WorkerId = worker.WorkerId,
                WorkDate = request.WorkDate.Date,
                Status = (byte)HousekeepingAssignmentStatus.Pending,
                Notes = request.Notes,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = null
            });
        }

        if (!newAssignments.Any())
        {
            return;
        }

        const string insertSql = @"INSERT INTO RoomHousekeepingAssignments
                                   (RoomId, WorkerId, WorkDate, ShiftCode, Status, Notes, CreatedOn, CreatedBy)
                                   VALUES (@RoomId, @WorkerId, @WorkDate, @ShiftCode, @Status, @Notes, @CreatedOn, @CreatedBy)";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(insertSql, newAssignments);
    }

    public async Task ReportIssueAsync(ReportIssueRequest request)
    {
        const string updateSql = @"UPDATE RoomHousekeepingAssignments
                                   SET Status = @Status,
                                       Notes = CASE WHEN ISNULL(Notes,'') = '' THEN @Note ELSE Notes + ' | ' + @Note END,
                                       ModifiedOn = SYSUTCDATETIME()
                                   WHERE WorkDate = @WorkDate AND RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(updateSql, new
        {
            Status = (byte)HousekeepingAssignmentStatus.Inspection,
            Note = $"Inspection: {request.Notes} ({request.ReportedBy ?? "Housekeeping"})",
            WorkDate = request.WorkDate.Date,
            RoomId = request.RoomId
        });
    }

    private async Task<List<RoomsWithAttribute>> FetchRoomsAsync()
    {
        try
        {
            var res = await _roomLockingAPIController.GetRoomWithCleaningAttributeById(0);
            if (res is ObjectResult objectResult && objectResult.Value is List<RoomsWithAttribute> dto)
            {
                return dto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch room snapshot for housekeeping dashboard");
        }
        return new List<RoomsWithAttribute>();
    }

    private async Task<Dictionary<string, RoomOccupancyIndicator>> BuildRoomOccupancyMapAsync(DateTime workDate)
    {
        var map = new Dictionary<string, RoomOccupancyIndicator>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var connection = _dbContext.CreateConnection();

            const string roomLockSql = @"SELECT Rooms,
                                                Type,
                                                TRY_CONVERT(date, FD) AS FromDate,
                                                TRY_CONVERT(date, ED) AS ToDate,
                                                ISNULL(Status,0) AS Status
                                         FROM RoomLock
                                         WHERE Rooms IS NOT NULL
                                           AND ISNULL(Status,0) = 1";

            var locks = await connection.QueryAsync<RoomLockSnapshot>(roomLockSql);
            foreach (var entry in locks)
            {
                if (!IsDateWithinRange(workDate, entry.FromDate, entry.ToDate))
                {
                    continue;
                }

                var indicator = string.Equals(entry.Type, "Hold", StringComparison.OrdinalIgnoreCase)
                    ? RoomOccupancyIndicator.OnHold
                    : RoomOccupancyIndicator.Locked;

                foreach (var roomNumber in SplitRoomNumbers(entry.Rooms))
                {
                    var normalized = NormalizeRoomNumber(roomNumber);
                    if (string.IsNullOrWhiteSpace(normalized))
                    {
                        continue;
                    }
                    map[normalized] = indicator;
                }
            }

            const string allocationSql = @"SELECT Rnumber AS RoomNumber,
                                                  CheckInDate,
                                                  CheckOutDate,
                                                  Fd,
                                                  Td,
                                                  ISNULL(Cancelled,0) AS Cancelled
                                           FROM RoomAllocation
                                           WHERE Rnumber IS NOT NULL
                                             AND ISNULL(IsActive,1) = 1
                                             AND (
                                                    (CheckInDate IS NOT NULL AND CAST(CheckInDate AS date) <= @WorkDate)
                                                 OR (Fd IS NOT NULL AND CAST(Fd AS date) <= @WorkDate)
                                                 )
                                             AND (
                                                    (CheckOutDate IS NOT NULL AND CAST(CheckOutDate AS date) >= @WorkDate)
                                                 OR (Td IS NOT NULL AND CAST(Td AS date) >= @WorkDate)
                                                 OR (CheckOutDate IS NULL AND Td IS NULL)
                                                 );";

            var allocations = await connection.QueryAsync<RoomAllocationSnapshot>(allocationSql, new { WorkDate = workDate.Date });
            foreach (var allocation in allocations)
            {
                var normalizedRoom = NormalizeRoomNumber(allocation.RoomNumber);
                if (string.IsNullOrWhiteSpace(normalizedRoom) || allocation.Cancelled)
                {
                    continue;
                }

                if (IsRoomOccupied(allocation, workDate))
                {
                    if (!map.TryGetValue(normalizedRoom, out var existing) ||
                        existing == RoomOccupancyIndicator.Empty)
                    {
                        map[normalizedRoom] = RoomOccupancyIndicator.Occupied;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build room occupancy snapshot");
        }

        return map;
    }

    private static IEnumerable<string> SplitRoomNumbers(string? rooms)
    {
        if (string.IsNullOrWhiteSpace(rooms))
        {
            return Array.Empty<string>();
        }
        return rooms
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(item => NormalizeRoomNumber(item))
            .Where(item => !string.IsNullOrWhiteSpace(item));
    }

    private static bool IsRoomOccupied(RoomAllocationSnapshot allocation, DateTime workDate)
    {
        var start = (allocation.CheckInDate ?? allocation.Fd)?.Date;
        var end = (allocation.CheckOutDate ?? allocation.Td)?.Date;

        if (start == null && end == null)
        {
            return false;
        }

        if (start != null && end != null)
        {
            var startDate = start.Value;
            var endDate = end.Value;
            if (endDate < startDate)
            {
                (startDate, endDate) = (endDate, startDate);
            }
            return workDate.Date >= startDate && workDate.Date <= endDate;
        }

        if (start != null && end == null)
        {
            return workDate.Date >= start.Value;
        }

        if (start == null && end != null)
        {
            return workDate.Date <= end.Value;
        }

        return false;
    }

    private static bool IsDateWithinRange(DateTime workDate, DateTime? start, DateTime? end)
    {
        if (start == null && end == null)
        {
            return false;
        }

        if (start == null)
        {
            return workDate.Date <= end!.Value.Date;
        }

        if (end == null)
        {
            return workDate.Date >= start.Value.Date;
        }

        var startDate = start.Value.Date;
        var endDate = end.Value.Date;
        if (endDate < startDate)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        return workDate.Date >= startDate && workDate.Date <= endDate;
    }

    private static string ResolveOccupancyLabel(RoomsWithAttribute room, Dictionary<string, RoomOccupancyIndicator> occupancyMap)
    {
        if (room == null)
        {
            return "Empty";
        }

        var key = NormalizeRoomNumber(room.Rnumber);
        if (!string.IsNullOrWhiteSpace(key) &&
            occupancyMap.TryGetValue(key, out var indicator))
        {
            return indicator switch
            {
                RoomOccupancyIndicator.Locked => "Locked",
                RoomOccupancyIndicator.OnHold => "On Hold",
                RoomOccupancyIndicator.Occupied => "Occupied",
                _ => "Empty"
            };
        }

        return "Empty";
    }

    private static string NormalizeRoomNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToUpperInvariant(ch));
            }
        }
        return builder.ToString();
    }

    private async Task<List<RoomHousekeepingAssignment>> FetchAssignmentsFromDbAsync(DateTime workDate)
    {
        const string sql = @"SELECT AssignmentId,
                                    RoomId,
                                    WorkerId,
                                    WorkDate,
                                    ShiftCode,
                                    Status,
                                    Notes,
                                    CreatedOn,
                                    CreatedBy,
                                    ModifiedOn,
                                    ModifiedBy
                             FROM RoomHousekeepingAssignments
                             WHERE WorkDate = @WorkDate";

        using var connection = _dbContext.CreateConnection();
        var rows = await connection.QueryAsync<RoomHousekeepingAssignment>(sql, new { WorkDate = workDate.Date });
        return rows.ToList();
    }

    private async Task<Dictionary<int, HousekeepingAssignmentState>> BuildAssignmentLookupAsync(DateTime workDate, Dictionary<int, HousekeepingWorkerSummary> rosterLookup)
    {
        var assignments = await FetchAssignmentsFromDbAsync(workDate);
        var lookup = new Dictionary<int, HousekeepingAssignmentState>();

        foreach (var assignment in assignments)
        {
            if (lookup.ContainsKey(assignment.RoomId))
            {
                continue;
            }
            var workerId = (int)assignment.WorkerId;
            rosterLookup.TryGetValue(workerId, out var worker);
            lookup[assignment.RoomId] = new HousekeepingAssignmentState
            {
                AssignmentId = assignment.AssignmentId,
                RoomId = assignment.RoomId,
                WorkerId = workerId,
                WorkerName = worker?.WorkerName ?? $"Worker #{workerId}",
                Status = (HousekeepingAssignmentStatus)assignment.Status,
                Notes = assignment.Notes,
                AssignedBy = worker?.EmployeeCode
            };
        }

        return lookup;
    }

    private static HousekeepingAssignmentStatus DetermineStatus(RoomsWithAttribute room, HousekeepingAssignmentState? assignment)
    {
        if (assignment != null)
        {
            return assignment.Status;
        }
        return room.CleanStatus ? HousekeepingAssignmentStatus.Completed : HousekeepingAssignmentStatus.Pending;
    }

    private static string DetermineOccupancy(RoomsWithAttribute room)
    {
        return room.Status switch
        {
            1 => "Occupied",
            2 => "On Hold",
            3 => "Locked",
            _ => "Empty"
        };
    }

    private async Task<List<HousekeepingWorkerSummary>> GetRosterAsync()
    {
        try
        {
            const string sql = @"SELECT 
                                    CAST(wm.WorkerID AS INT) AS WorkerId,
                                    ISNULL(wm.WorkerName, 
                                        LTRIM(RTRIM(
                                            ISNULL(wm.FirstName, '') + 
                                            CASE WHEN wm.MiddleName IS NOT NULL AND wm.MiddleName <> '' THEN ' ' + wm.MiddleName ELSE '' END +
                                            CASE WHEN wm.LastName IS NOT NULL AND wm.LastName <> '' THEN ' ' + wm.LastName ELSE '' END
                                        ))
                                    ) AS WorkerName,
                                    ISNULL(wm.EMPID, '') AS EmployeeCode,
                                    ISNULL(rm.RoleName, 'Unassigned') AS Department
                                FROM EHRMS.dbo.WorkerMaster wm WITH (NOLOCK)
                                LEFT JOIN EHRMS.dbo.RoleMaster rm WITH (NOLOCK) ON wm.RoleID = rm.RoleID
                                WHERE ISNULL(wm.isactive, 'Y') = 'Y'
                                    AND wm.WorkerID IS NOT NULL
                                ORDER BY rm.RoleName, wm.WorkerName";

            using var connection = _ehrmsDbContext.CreateConnection();
            var data = await connection.QueryAsync<HousekeepingWorkerSummary>(sql);
            var roster = data.ToList();
            
            // Ensure WorkerId is properly set (handle decimal to int conversion)
            foreach (var worker in roster)
            {
                if (worker.WorkerId == 0)
                {
                    _logger.LogWarning($"Worker with invalid WorkerID found: {worker.WorkerName}");
                }
            }
            
            return roster.Where(w => w.WorkerId > 0).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch worker roster from EHRMS");
            // Return empty list on error to prevent dashboard from breaking
            return new List<HousekeepingWorkerSummary>();
        }
    }

    private async Task<List<HousekeepingDepartmentOption>> GetDepartmentsAsync()
    {
        try
        {
            const string sql = @"SELECT DISTINCT rm.RoleID AS Id, rm.RoleName AS Name
                                 FROM EHRMS.dbo.RoleMaster rm WITH (NOLOCK)
                                 WHERE ISNULL(rm.isActive, 'Y') = 'Y' AND ISNULL(rm.RoleName, '') <> ''
                                 ORDER BY rm.RoleName";

            using var connection = _ehrmsDbContext.CreateConnection();
            var data = await connection.QueryAsync<HousekeepingDepartmentOption>(sql);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch departments from EHRMS");
            return new List<HousekeepingDepartmentOption>();
        }
    }

    private class HousekeepingAssignmentState
    {
        public int AssignmentId { get; set; }
        public int RoomId { get; set; }
        public int WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public HousekeepingAssignmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? AssignedBy { get; set; }
    }

    private class RoomAllocationSnapshot
    {
        public string? RoomNumber { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime? Fd { get; set; }
        public DateTime? Td { get; set; }
        public bool Cancelled { get; set; }
    }

    private class RoomLockSnapshot
    {
        public string? Rooms { get; set; }
        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    private enum RoomOccupancyIndicator
    {
        Empty = 0,
        Occupied = 1,
        Locked = 2,
        OnHold = 3
    }

    private class RoomChecklistReviewRow
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RChkLstID { get; set; }
        public DateTime? ChkDate { get; set; }
        public int? CheckedBy { get; set; }
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public string? ReviewedByName { get; set; }
    }

    private static List<int> ParseChecklistIds(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return new List<int>();
        }

        return csv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(segment => int.TryParse(segment, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
    }

        private async Task SaveRoomChecklistAsync(HousekeeperCleanRequest request, int workerId)
        {
            if (request.ChecklistItemIds == null || !request.ChecklistItemIds.Any())
            {
                throw new InvalidOperationException("Please select at least one checklist item.");
            }

            var payload = new RoomChkListDTO
            {
                RID = request.RoomId,
                RChkLstID = string.Join(",", request.ChecklistItemIds),
                Reason = request.Reason,
                Comments = request.Comments,
                CheckedBy = workerId,
                ChkDate = DateTime.UtcNow
            };

            var res = await _roomLockingAPIController.RoomCleanCheckList(payload);
            if (res is ObjectResult objectResult &&
                objectResult.StatusCode.HasValue &&
                objectResult.StatusCode != StatusCodes.Status200OK)
            {
                var message = ExtractErrorMessage(objectResult.Value, "Failed to save cleaning checklist.");
                throw new InvalidOperationException(message);
            }
            if (res is BadRequestObjectResult badRequest)
            {
                var message = ExtractErrorMessage(badRequest.Value, "Cleaning checklist validation failed.");
                throw new InvalidOperationException(message);
            }
        }

        private async Task UpdateAssignmentCompletionAsync(int roomId, DateTime workDate, int workerId, string? notes)
        {
            const string sql = @"UPDATE RoomHousekeepingAssignments
                                 SET Status = @Status,
                                     Notes = CASE WHEN @Notes IS NULL OR LTRIM(RTRIM(@Notes)) = '' THEN Notes ELSE @Notes END,
                                     ModifiedOn = SYSUTCDATETIME(),
                                     ModifiedBy = @WorkerId
                                 WHERE RoomId = @RoomId AND WorkDate = @WorkDate AND WorkerId = @WorkerId";

            using var connection = _dbContext.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new
            {
                Status = (byte)HousekeepingAssignmentStatus.Completed,
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
                WorkerId = workerId,
                RoomId = roomId,
                WorkDate = workDate.Date
            });

            if (affected == 0)
            {
                throw new InvalidOperationException("Assignment not found for the selected room.");
            }
        }

        private static string ExtractErrorMessage(object? payload, string fallback)
        {
            if (payload == null)
            {
                return fallback;
            }

            if (payload is string text && !string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var type = payload.GetType();
            var property = type.GetProperty("message", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property?.GetValue(payload) is string message && !string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            var toStringValue = payload.ToString();
            return string.IsNullOrWhiteSpace(toStringValue) ? fallback : toStringValue!;
        }
}

public class AssignRoomsRequest
{
    public int WorkerId { get; set; }
    public DateTime WorkDate { get; set; }
    public List<int>? RoomIds { get; set; }
    public string? Notes { get; set; }
    public string? AssignedBy { get; set; }
}

public class ReportIssueRequest
{
    public int RoomId { get; set; }
    public DateTime WorkDate { get; set; }
    public string? Notes { get; set; }
    public string? ReportedBy { get; set; }
}

public class HousekeeperCleanRequest
{
    public int RoomId { get; set; }
    public DateTime WorkDate { get; set; }
    public List<int>? ChecklistItemIds { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
}

