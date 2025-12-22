using System.Security.Claims;
using GMS.Core.Entities;
using GMS.Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers;

[ApiController]
[Route("api/tasks-assigned")]
public class TasksAssignedAPIController : ControllerBase
{
    private readonly ILogger<TasksAssignedAPIController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TasksAssignedAPIController(ILogger<TasksAssignedAPIController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public class MarkStartRequest
    {
        public int ScheduleId { get; set; }
    }

    public class MarkCompleteRequest
    {
        public int ScheduleId { get; set; }
    }

    public class ReportIssueRequest
    {
        public int ScheduleId { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("mark-start")]
    public async Task<IActionResult> MarkStart([FromBody] MarkStartRequest request)
    {
        var workerIdClaim = User.FindFirstValue("WorkerId");
        if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var workerId) || workerId <= 0)
        {
            return Unauthorized("Unable to determine worker identity.");
        }

        if (request == null || request.ScheduleId <= 0)
        {
            return BadRequest("ScheduleId is required.");
        }

        try
        {
            var now = DateTime.Now;
            const string selectQuery = @"Select top 1 * from EmployeeTaskExecution 
                                         where GuestScheduledTaskId=@ScheduleId and EmployeeId=@EmployeeId
                                         order by Id desc";
            var param = new { ScheduleId = request.ScheduleId, EmployeeId = workerId };
            var existing = (await _unitOfWork.EmployeeTaskExecution.GetTableData<EmployeeTaskExecution>(selectQuery, param))?.FirstOrDefault();

            if (existing == null)
            {
                var entity = new EmployeeTaskExecution
                {
                    GuestScheduledTaskId = request.ScheduleId,
                    EmployeeId = workerId,
                    ActualStartTime = now,
                    ExecutionStatus = "Started",
                    CreatedOn = now,
                    UpdatedOn = now
                };

                entity.Id = await _unitOfWork.EmployeeTaskExecution.AddAsync(entity);
                return Ok(new
                {
                    success = true,
                    startedAt = entity.ActualStartTime,
                    id = entity.Id
                });
            }
            else
            {
                if (existing.ActualStartTime == null)
                {
                    existing.ActualStartTime = now;
                }
                existing.ExecutionStatus = "Started";
                existing.UpdatedOn = now;

                await _unitOfWork.EmployeeTaskExecution.UpdateAsync(existing);

                return Ok(new
                {
                    success = true,
                    startedAt = existing.ActualStartTime,
                    id = existing.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark task as started for schedule {ScheduleId}", request.ScheduleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to mark as started right now." });
        }
    }

    [HttpPost("mark-complete")]
    public async Task<IActionResult> MarkComplete([FromBody] MarkCompleteRequest request)
    {
        var workerIdClaim = User.FindFirstValue("WorkerId");
        if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var workerId) || workerId <= 0)
        {
            return Unauthorized("Unable to determine worker identity.");
        }

        if (request == null || request.ScheduleId <= 0)
        {
            return BadRequest("ScheduleId is required.");
        }

        try
        {
            var now = DateTime.Now;
            const string selectQuery = @"Select top 1 * from EmployeeTaskExecution 
                                         where GuestScheduledTaskId=@ScheduleId and EmployeeId=@EmployeeId
                                         order by Id desc";
            var param = new { ScheduleId = request.ScheduleId, EmployeeId = workerId };
            var existing = (await _unitOfWork.EmployeeTaskExecution.GetTableData<EmployeeTaskExecution>(selectQuery, param))?.FirstOrDefault();

            if (existing == null)
            {
                var entity = new EmployeeTaskExecution
                {
                    GuestScheduledTaskId = request.ScheduleId,
                    EmployeeId = workerId,
                    ActualStartTime = now,
                    ActualEndTime = now,
                    ExecutionStatus = "Completed",
                    CreatedOn = now,
                    UpdatedOn = now
                };

                entity.Id = await _unitOfWork.EmployeeTaskExecution.AddAsync(entity);
                return Ok(new
                {
                    success = true,
                    completedAt = entity.ActualEndTime,
                    startedAt = entity.ActualStartTime,
                    id = entity.Id
                });
            }
            else
            {
                if (existing.ActualStartTime == null)
                {
                    existing.ActualStartTime = now;
                }
                existing.ActualEndTime = now;
                existing.ExecutionStatus = "Completed";
                existing.UpdatedOn = now;

                await _unitOfWork.EmployeeTaskExecution.UpdateAsync(existing);

                return Ok(new
                {
                    success = true,
                    completedAt = existing.ActualEndTime,
                    startedAt = existing.ActualStartTime,
                    id = existing.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark task as completed for schedule {ScheduleId}", request.ScheduleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to mark as completed right now." });
        }
    }

    [HttpPost("report-issue")]
    public async Task<IActionResult> ReportIssue([FromBody] ReportIssueRequest request)
    {
        var workerIdClaim = User.FindFirstValue("WorkerId");
        if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var workerId) || workerId <= 0)
        {
            return Unauthorized("Unable to determine worker identity.");
        }

        if (request == null || request.ScheduleId <= 0)
        {
            return BadRequest("ScheduleId is required.");
        }

        try
        {
            var now = DateTime.Now;
            const string selectQuery = @"Select top 1 * from EmployeeTaskExecution 
                                         where GuestScheduledTaskId=@ScheduleId and EmployeeId=@EmployeeId
                                         order by Id desc";
            var param = new { ScheduleId = request.ScheduleId, EmployeeId = workerId };
            var existing = (await _unitOfWork.EmployeeTaskExecution.GetTableData<EmployeeTaskExecution>(selectQuery, param))?.FirstOrDefault();

            if (existing == null)
            {
                var entity = new EmployeeTaskExecution
                {
                    GuestScheduledTaskId = request.ScheduleId,
                    EmployeeId = workerId,
                    ExecutionStatus = "Inspection",
                    IssueReportedBit = true,
                    IssueNotes = request.Notes,
                    CreatedOn = now,
                    UpdatedOn = now
                };

                entity.Id = await _unitOfWork.EmployeeTaskExecution.AddAsync(entity);
                return Ok(new
                {
                    success = true,
                    id = entity.Id
                });
            }
            else
            {
                existing.ExecutionStatus = "Inspection";
                existing.IssueReportedBit = true;
                existing.IssueNotes = request.Notes;
                existing.UpdatedOn = now;

                await _unitOfWork.EmployeeTaskExecution.UpdateAsync(existing);

                return Ok(new
                {
                    success = true,
                    id = existing.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report issue for schedule {ScheduleId}", request.ScheduleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to report issue right now." });
        }
    }
}

