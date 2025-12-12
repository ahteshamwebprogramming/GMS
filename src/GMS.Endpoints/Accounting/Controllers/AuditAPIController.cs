using AutoMapper;
using Dapper;
using GMS.Core.Repository;
using GMS.Core.Entities;
using GMS.Infrastructure.Models.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;

namespace GMS.Endpoints.Accounting;

[Route("api/[controller]")]
[ApiController]
public class AuditAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditAPIController> _logger;
    private readonly IMapper _mapper;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public AuditAPIController(IUnitOfWork unitOfWork, ILogger<AuditAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv, IServiceScopeFactory serviceScopeFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
        _serviceScopeFactory = serviceScopeFactory;
    }
    [HttpGet("status")]
    public async Task<IActionResult> GetNightAuditStatus()
    {
        try
        {
            const string query = @"SELECT TOP(1)
                                        nal.RunDate,
                                        nal.LastAuditDate,
                                        wm.WorkerName AS OperatorName
                                   FROM NightAuditLog nal
                                   LEFT JOIN EHRMS.dbo.WorkerMaster wm ON wm.WorkerID = nal.RunBy
                                   ORDER BY nal.RunDate DESC, nal.Id DESC";

            var result = await _unitOfWork.GMSFinalGuest.GetTableData<NightAuditStatusDTO>(query);
            var status = result?.FirstOrDefault();

            return Ok(status ?? new NightAuditStatusDTO());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving night audit status {nameof(GetNightAuditStatus)}");
            throw;
        }
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunAudit(int UserId)
    {
        try
        {
            // Get current audit status before starting
            var currentStatus = await GetCurrentAuditStatus();
            var initialRunDate = currentStatus?.RunDate;

            _logger.LogInformation($"Starting RunNightAudit in background for UserId: {UserId}");

            // Start the audit in background - fire and forget
            // Use a separate scope to avoid disposed context issues
            _ = Task.Run(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        
                        string sp = "RunNightAudit";
                        var param = new DynamicParameters();
                        param.Add("@UserId", UserId);
                        param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        
                        // Set command timeout to 30 minutes for background execution
                        int commandTimeout = 1800; // 30 minutes
                        
                        var res = await unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync(sp, param, commandTimeout);
                        
                        int result = param.Get<int>("@Status");
                        if (result > 0)
                        {
                            _logger.LogInformation($"RunNightAudit completed successfully for UserId: {UserId}. Status: {result}");
                            
                            // Update the RunBy field in the most recent NightAuditLog record
                            try
                            {
                                const string updateQuery = @"UPDATE NightAuditLog 
                                                             SET RunBy = @UserId 
                                                             WHERE Id = (SELECT TOP(1) Id FROM NightAuditLog ORDER BY RunDate DESC, Id DESC)";
                                var updateParams = new DynamicParameters();
                                updateParams.Add("@UserId", UserId);
                                await unitOfWork.GMSFinalGuest.ExecuteQuery(updateQuery, updateParams);
                                _logger.LogInformation($"Updated NightAuditLog RunBy field with UserId: {UserId}");
                            }
                            catch (Exception updateEx)
                            {
                                _logger.LogWarning(updateEx, $"Failed to update RunBy in NightAuditLog for UserId: {UserId}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"RunNightAudit completed with status 0 for UserId: {UserId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing stored procedure 'RunNightAudit' in background for UserId: {UserId}");
                    }
                }
            });

            // Return immediately - audit is running in background
            return Ok(new { message = "Audit started successfully", initialRunDate = initialRunDate });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error starting RunNightAudit for UserId: {UserId}");
            return StatusCode(500, new { message = "Error starting stored procedure 'RunNightAudit'", error = ex.Message });
        }
    }

    private async Task<NightAuditStatusDTO?> GetCurrentAuditStatus()
    {
        try
        {
            const string query = @"SELECT TOP(1)
                                        nal.RunDate,
                                        nal.LastAuditDate,
                                        wm.WorkerName AS OperatorName
                                   FROM NightAuditLog nal
                                   LEFT JOIN EHRMS.dbo.WorkerMaster wm ON wm.WorkerID = nal.RunBy
                                   ORDER BY nal.RunDate DESC, nal.Id DESC";

            var result = await _unitOfWork.GMSFinalGuest.GetTableData<NightAuditStatusDTO>(query);
            return result?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
