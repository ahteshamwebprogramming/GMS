using GMS.Infrastructure.ViewModels.Rooms;
using GMS.WebUI.Services.Rooms;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace GMS.WebUI.Controllers.Rooms;

[Route("api/[controller]")]
[ApiController]
public class RoomHousekeepingAPIController : ControllerBase
{
    private readonly RoomHousekeepingDashboardService _dashboardService;
    private readonly ILogger<RoomHousekeepingAPIController> _logger;

    public RoomHousekeepingAPIController(RoomHousekeepingDashboardService dashboardService,
        ILogger<RoomHousekeepingAPIController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] DateTime? workDate)
    {
        var targetDate = workDate ?? DateTime.Today;
        var dashboard = await _dashboardService.GetDashboardAsync(targetDate);
        return Ok(dashboard);
    }

    [HttpGet("unassigned")]
    public async Task<IActionResult> GetUnassigned([FromQuery] DateTime? workDate)
    {
        var rooms = await _dashboardService.GetUnassignedRoomsAsync(workDate ?? DateTime.Today);
        return Ok(rooms);
    }

    [HttpGet("worker-assignments")]
    public async Task<IActionResult> GetWorkerAssignments([FromQuery] DateTime? workDate, [FromQuery] int workerId)
    {
        var assignments = await _dashboardService.GetWorkerAssignmentsAsync(workDate ?? DateTime.Today, workerId);
        return Ok(assignments);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRooms([FromBody] AssignRoomsRequest request)
    {
        try
        {
            if (request.RoomIds == null || !request.RoomIds.Any())
            {
                return BadRequest("Please select at least one room.");
            }
            await _dashboardService.AssignRoomsAsync(request);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign rooms");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("report-issue")]
    public async Task<IActionResult> ReportIssue([FromBody] ReportIssueRequest request)
    {
        await _dashboardService.ReportIssueAsync(request);
        return Ok(new { success = true });
    }

    [HttpGet("checklist")]
    public async Task<IActionResult> GetChecklist()
    {
        var checklist = await _dashboardService.GetRoomChecklistAsync();
        return Ok(checklist);
    }

    [HttpGet("checklist/review")]
    public async Task<IActionResult> GetChecklistReview([FromQuery] int roomId)
    {
        if (roomId <= 0)
        {
            return BadRequest("RoomId is required.");
        }

        var review = await _dashboardService.GetRoomChecklistReviewAsync(roomId);
        if (review == null)
        {
            return NotFound();
        }

        return Ok(review);
    }

    [HttpPost("mark-clean")]
    public async Task<IActionResult> MarkClean([FromBody] HousekeeperCleanRequest request)
    {
        var workerIdClaim = User.FindFirstValue("WorkerId");
        if (string.IsNullOrWhiteSpace(workerIdClaim) || !int.TryParse(workerIdClaim, out var workerId) || workerId <= 0)
        {
            return Unauthorized("Unable to determine worker identity.");
        }

        try
        {
            await _dashboardService.MarkRoomCleanAsync(request, workerId);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation failed while marking room clean.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while marking room clean.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to mark room as clean right now. Please try again." });
        }
    }
}

