using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Home;
using GMS.Infrastructure.ViewModels.ResourceAllocation;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Guests;

[Route("api/[controller]")]
[ApiController]
public class TasksAssignedAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TasksAssignedAPIController> _logger;
    private readonly IConfiguration _configuration;

    public TasksAssignedAPIController(IUnitOfWork unitOfWork, ILogger<TasksAssignedAPIController> logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
    }

    private string GetEHRMSDatabaseName()
    {
        var connectionString = _configuration.GetConnectionString("EHRMSConnectionDB");
        if (string.IsNullOrEmpty(connectionString))
        {
            return "EHRMS";
        }

        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.Trim().StartsWith("Initial Catalog", StringComparison.OrdinalIgnoreCase) ||
                part.Trim().StartsWith("Database", StringComparison.OrdinalIgnoreCase))
            {
                var dbName = part.Split('=')[1]?.Trim();
                return dbName ?? "EHRMS";
            }
        }

        return "EHRMS";
    }

    [HttpGet("GetTeamMembers")]
    public async Task<IActionResult> GetTeamMembers(int workerId)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();
            string teamQuery = $@"SELECT 
                                    CAST(wm.WorkerID AS INT) AS Id,
                                    ISNULL(wm.WorkerName, 
                                        LTRIM(RTRIM(
                                            ISNULL(wm.FirstName, '') + 
                                            CASE WHEN wm.MiddleName IS NOT NULL AND wm.MiddleName <> '' THEN ' ' + wm.MiddleName ELSE '' END +
                                            CASE WHEN wm.LastName IS NOT NULL AND wm.LastName <> '' THEN ' ' + wm.LastName ELSE '' END
                                        ))
                                    ) AS Name
                                FROM [{ehrmsDbName}].dbo.WorkerMaster wm
                                WHERE wm.RoleID = (SELECT RoleID FROM [{ehrmsDbName}].dbo.WorkerMaster WHERE WorkerID = @WorkerId)
                                AND wm.IsActive = 1
                                ORDER BY Name";
            var teamParam = new { WorkerId = workerId };
            var teamMembers = await _unitOfWork.GenOperations.GetTableData<EmployeeOption>(teamQuery, teamParam);

            return Ok(teamMembers ?? new List<EmployeeOption>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch team members for worker {WorkerId}", workerId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to fetch team members." });
        }
    }

    [HttpGet("GetWorkerInfo")]
    public async Task<IActionResult> GetWorkerInfo(int workerId)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();
            string workerQuery = $@"Select 
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
                            from [{ehrmsDbName}].dbo.WorkerMaster wm
                            LEFT JOIN [{ehrmsDbName}].dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                            where wm.WorkerID = @WorkerId";
            var workerParam = new { WorkerId = workerId };
            var workerInfo = await _unitOfWork.GenOperations.GetEntityData<HousekeepingWorkerSummary>(workerQuery, workerParam);

            return Ok(workerInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch worker info for worker {WorkerId}", workerId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to fetch worker info." });
        }
    }

    [HttpGet("GetTasksAssigned")]
    public async Task<IActionResult> GetTasksAssigned(DateTime workDate, int workerId)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();

            string query = $@"Select
                            gs.Id,
                            gs.GuestId,
                            gs.StartDateTime,
                            gs.EndDateTime,
                            gs.Duration,
                            gs.TaskId,
                            gs.EmployeeId1,
                            gs.EmployeeId2,
                            gs.EmployeeId3,
                            gs.SessionId,
                            gs.ResourceId,
                            tm.TaskName,
                            rm.ResourceName,
                            wm1.WorkerName Therapist1Name,
                            wm2.WorkerName Therapist2Name,
                            wm3.WorkerName Therapist3Name,
                            md.CustomerName GuestName,
                            ra.Rnumber RoomNo
                            from GuestSchedule gs
                            Left Join TaskMaster tm on gs.TaskId=tm.Id
                            Left Join ResourceMaster rm on gs.ResourceId=rm.Id
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm1 on gs.EmployeeId1=wm1.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm2 on gs.EmployeeId2=wm2.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm3 on gs.EmployeeId3=wm3.WorkerID
                            Left Join MembersDetails md on gs.GuestId=md.Id
                            Left Join RoomAllocation ra on gs.GuestId=ra.GuestID
                            where (gs.EmployeeId1 = @WorkerId OR gs.EmployeeId2 = @WorkerId OR gs.EmployeeId3 = @WorkerId)
                            AND CAST(gs.StartDateTime AS DATE) = CAST(@WorkDate AS DATE)
                            AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                            AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)
                            order by gs.StartDateTime asc";

            var parameters = new { WorkerId = workerId, WorkDate = workDate };
            var schedules = await _unitOfWork.GenOperations.GetTableData<GuestScheduleWithAttributes>(query, parameters);

            return Ok(schedules ?? new List<GuestScheduleWithAttributes>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch tasks assigned for worker {WorkerId}", workerId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to fetch tasks assigned." });
        }
    }

    [HttpGet("GetTaskExecution")]
    public async Task<IActionResult> GetTaskExecution(int scheduleId, int employeeId)
    {
        try
        {
            const string execQuery = @"select top 1 * from EmployeeTaskExecution 
                                       where GuestScheduledTaskId=@ScheduleId and EmployeeId=@EmployeeId
                                       order by Id desc";
            var execParam = new { ScheduleId = scheduleId, EmployeeId = employeeId };
            var execution = await _unitOfWork.EmployeeTaskExecution.GetEntityData<EmployeeTaskExecution>(execQuery, execParam);

            return Ok(execution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch task execution for schedule {ScheduleId}", scheduleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to fetch task execution." });
        }
    }

    [HttpGet("GetTasksAssignedViewModel")]
    public async Task<IActionResult> GetTasksAssignedViewModel(DateTime workDate, int workerId, int? loggedInWorkerId = null)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();

            // Get schedules
            string query = $@"Select
                            gs.Id,
                            gs.GuestId,
                            gs.StartDateTime,
                            gs.EndDateTime,
                            gs.Duration,
                            gs.TaskId,
                            gs.EmployeeId1,
                            gs.EmployeeId2,
                            gs.EmployeeId3,
                            gs.SessionId,
                            gs.ResourceId,
                            tm.TaskName,
                            rm.ResourceName,
                            wm1.WorkerName Therapist1Name,
                            wm2.WorkerName Therapist2Name,
                            wm3.WorkerName Therapist3Name,
                            md.CustomerName GuestName,
                            ra.Rnumber RoomNo
                            from GuestSchedule gs
                            Left Join TaskMaster tm on gs.TaskId=tm.Id
                            Left Join ResourceMaster rm on gs.ResourceId=rm.Id
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm1 on gs.EmployeeId1=wm1.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm2 on gs.EmployeeId2=wm2.WorkerID
                            Left Join [{ehrmsDbName}].dbo.WorkerMaster wm3 on gs.EmployeeId3=wm3.WorkerID
                            Left Join MembersDetails md on gs.GuestId=md.Id
                            Left Join RoomAllocation ra on gs.GuestId=ra.GuestID
                            where (gs.EmployeeId1 = @WorkerId OR gs.EmployeeId2 = @WorkerId OR gs.EmployeeId3 = @WorkerId)
                            AND CAST(gs.StartDateTime AS DATE) = CAST(@WorkDate AS DATE)
                            AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                            AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)
                            order by gs.StartDateTime asc";

            var parameters = new { WorkerId = workerId, WorkDate = workDate };
            var schedules = await _unitOfWork.GenOperations.GetTableData<GuestScheduleWithAttributes>(query, parameters);

            // Get worker details
            string workerQuery = $@"Select 
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
                            from [{ehrmsDbName}].dbo.WorkerMaster wm
                            LEFT JOIN [{ehrmsDbName}].dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                            where wm.WorkerID = @WorkerId";
            var workerParam = new { WorkerId = workerId };
            var workerInfo = await _unitOfWork.GenOperations.GetEntityData<HousekeepingWorkerSummary>(workerQuery, workerParam);

            // Get team members with same role
            var roleWorkerId = loggedInWorkerId ?? workerId;
            string teamQuery = $@"SELECT 
                                CAST(wm.WorkerID AS INT) AS Id,
                                ISNULL(wm.WorkerName, 
                                    LTRIM(RTRIM(
                                        ISNULL(wm.FirstName, '') + 
                                        CASE WHEN wm.MiddleName IS NOT NULL AND wm.MiddleName <> '' THEN ' ' + wm.MiddleName ELSE '' END +
                                        CASE WHEN wm.LastName IS NOT NULL AND wm.LastName <> '' THEN ' ' + wm.LastName ELSE '' END
                                    ))
                                ) AS Name
                            FROM [{ehrmsDbName}].dbo.WorkerMaster wm
                            WHERE wm.RoleID = (SELECT RoleID FROM [{ehrmsDbName}].dbo.WorkerMaster WHERE WorkerID = @RoleWorkerId)
                            AND wm.IsActive = 'Y'
                            ORDER BY Name";
            var teamParam = new { RoleWorkerId = roleWorkerId };
            var teamMembers = (await _unitOfWork.GenOperations.GetTableData<EmployeeOption>(teamQuery, teamParam))?.ToList()
                ?? new List<EmployeeOption>();

            // Get logged-in worker name
            string? loggedInWorkerName = null;
            if (loggedInWorkerId.HasValue && loggedInWorkerId.Value != workerId)
            {
                var loggedInWorkerInfo = await _unitOfWork.GenOperations.GetEntityData<HousekeepingWorkerSummary>(workerQuery, new { WorkerId = loggedInWorkerId.Value });
                loggedInWorkerName = loggedInWorkerInfo?.WorkerName;
            }
            else if (loggedInWorkerId.HasValue && loggedInWorkerId.Value == workerId)
            {
                loggedInWorkerName = workerInfo?.WorkerName;
            }

            // Map to view model
            var viewModel = new TasksAssignedViewModel
            {
                WorkDate = workDate,
                WorkerId = workerId,
                WorkerName = workerInfo?.WorkerName ?? "Employee",
                EmployeeCode = workerInfo?.EmployeeCode,
                Department = workerInfo?.Department,
                LoggedInWorkerId = loggedInWorkerId ?? workerId,
                LoggedInWorkerName = loggedInWorkerName ?? "Employee",
                Assignments = new List<HousekeepingAssignmentRow>(),
                TeamMembers = teamMembers
            };

            // Map GuestSchedule data to HousekeepingAssignmentRow structure
            foreach (var schedule in schedules ?? new List<GuestScheduleWithAttributes>())
            {
                var status = HousekeepingAssignmentStatus.Pending;
                DateTime? actualStart = null;
                DateTime? actualEnd = null;
                string? execStatus = null;
                bool issueReported = false;
                string? issueNotes = null;

                try
                {
                    const string execQuery = @"select top 1 * from EmployeeTaskExecution 
                                               where GuestScheduledTaskId=@ScheduleId and EmployeeId=@EmployeeId
                                               order by Id desc";
                    var execParam = new { ScheduleId = schedule.Id, EmployeeId = workerId };
                    var execution = await _unitOfWork.EmployeeTaskExecution.GetEntityData<EmployeeTaskExecution>(execQuery, execParam);
                    if (execution != null)
                    {
                        actualStart = execution.ActualStartTime;
                        actualEnd = execution.ActualEndTime;
                        execStatus = execution.ExecutionStatus;
                        issueReported = execution.IssueReportedBit ?? false;
                        issueNotes = execution.IssueNotes;

                        if (!string.IsNullOrWhiteSpace(execStatus) && execStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                        {
                            status = HousekeepingAssignmentStatus.Completed;
                        }
                        else if (!string.IsNullOrWhiteSpace(execStatus) && execStatus.Equals("Inspection", StringComparison.OrdinalIgnoreCase))
                        {
                            status = HousekeepingAssignmentStatus.Inspection;
                        }
                        else if (actualEnd.HasValue)
                        {
                            status = HousekeepingAssignmentStatus.Completed;
                        }
                        else if (actualStart.HasValue)
                        {
                            status = HousekeepingAssignmentStatus.InProgress;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to read execution state for schedule {ScheduleId}", schedule.Id);
                }

                viewModel.Assignments.Add(new HousekeepingAssignmentRow
                {
                    ScheduleId = schedule.Id,
                    RoomId = schedule.GuestId,
                    RoomNumber = schedule.RoomNo ?? "N/A",
                    RoomType = schedule.TaskName ?? "Task",
                    OccupancyStatus = "Scheduled",
                    IsAssigned = true,
                    AssignedTo = schedule.Therapist1Name ?? "Unassigned",
                    Status = status,
                    Notes = $"Guest: {schedule.GuestName ?? "N/A"} | Resource: {schedule.ResourceName ?? "N/A"}",
                    StartDateTime = schedule.StartDateTime,
                    EndDateTime = schedule.EndDateTime,
                    Duration = schedule.Duration,
                    ActualStartTime = actualStart,
                    ActualEndTime = actualEnd,
                    IssueReported = issueReported,
                    IssueNotes = issueNotes,
                    GuestName = schedule.GuestName,
                    TreatmentRoom = schedule.ResourceName,
                    OtherAssignees = string.Join(", ", new[] { schedule.Therapist2Name, schedule.Therapist3Name }
                        .Where(n => !string.IsNullOrWhiteSpace(n)))
                });
            }

            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tasks assigned for worker {WorkerId}", workerId);
            return Ok(new TasksAssignedViewModel
            {
                WorkDate = workDate,
                WorkerName = "Employee",
                Assignments = new List<HousekeepingAssignmentRow>()
            });
        }
    }
}
