using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class OperationsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OperationsAPIController> _logger;
    private readonly IMapper _mapper;
    
    public OperationsAPIController(IUnitOfWork unitOfWork, ILogger<OperationsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            string query = "SELECT TOP 1 * FROM Operations WHERE IsActive = 1 ORDER BY Id DESC";
            var result = await _unitOfWork.Operations.GetEntityData<OperationsDTO>(query);
            
            if (result == null)
            {
                // Return default empty object if no record exists
                return Ok(new OperationsDTO 
                { 
                    IsActive = true,
                    SmtpPort = 587,
                    SmtpEnableSsl = true
                });
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving Operations {nameof(Get)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OperationsDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Operations data is required");
            }

            // Check if a record already exists
            string existingQuery = "SELECT TOP 1 * FROM Operations WHERE IsActive = 1 ORDER BY Id DESC";
            var existing = await _unitOfWork.Operations.GetEntityData<Operations>(existingQuery);
            
            if (existing != null)
            {
                // Update existing record - update all fields from DTO
                existing.LogoFilePath = dto.LogoFilePath;
                existing.SmtpServer = dto.SmtpServer;
                existing.SmtpPort = dto.SmtpPort;
                existing.SmtpUsername = dto.SmtpUsername;
                existing.SmtpPassword = dto.SmtpPassword;
                existing.SmtpEnableSsl = dto.SmtpEnableSsl;
                existing.SmtpFromEmail = dto.SmtpFromEmail;
                existing.SmtpFromName = dto.SmtpFromName;
                existing.FeedbackWelcomeTitle = dto.FeedbackWelcomeTitle;
                existing.FeedbackWelcomeMessage1 = dto.FeedbackWelcomeMessage1;
                existing.FeedbackWelcomeMessage2 = dto.FeedbackWelcomeMessage2;
                existing.FeedbackWelcomeMessage3 = dto.FeedbackWelcomeMessage3;
                existing.ModifiedDate = DateTime.UtcNow;
                existing.ModifiedBy = dto.ModifiedBy;
                existing.IsActive = dto.IsActive;

                var updated = await _unitOfWork.Operations.UpdateAsync(existing);
                if (updated)
                {
                    return Ok(_mapper.Map<OperationsDTO>(existing));
                }
                return BadRequest("Unable to update Operations");
            }
            else
            {
                // Create new record
                dto.CreatedDate = DateTime.UtcNow;
                dto.IsActive = true;
                var entity = _mapper.Map<Operations>(dto);
                dto.Id = await _unitOfWork.Operations.AddAsync(entity);
                
                if (dto.Id > 0)
                {
                    return Ok(dto);
                }
                return BadRequest("Unable to create Operations");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving Operations {nameof(Post)}");
            return StatusCode(500, new { message = $"Error saving Operations: {ex.Message}" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] OperationsDTO dto)
    {
        try
        {
            if (dto == null || id != dto.Id)
            {
                return BadRequest("Invalid Operations data");
            }

            string query = "SELECT * FROM Operations WHERE Id = @Id";
            var param = new { @Id = id };
            var existing = await _unitOfWork.Operations.GetEntityData<Operations>(query, param);
            
            if (existing == null)
            {
                return NotFound("Operations not found");
            }

            existing.LogoFilePath = dto.LogoFilePath;
            existing.SmtpServer = dto.SmtpServer;
            existing.SmtpPort = dto.SmtpPort;
            existing.SmtpUsername = dto.SmtpUsername;
            existing.SmtpPassword = dto.SmtpPassword;
            existing.SmtpEnableSsl = dto.SmtpEnableSsl;
            existing.SmtpFromEmail = dto.SmtpFromEmail;
            existing.SmtpFromName = dto.SmtpFromName;
            existing.FeedbackWelcomeTitle = dto.FeedbackWelcomeTitle;
            existing.FeedbackWelcomeMessage1 = dto.FeedbackWelcomeMessage1;
            existing.FeedbackWelcomeMessage2 = dto.FeedbackWelcomeMessage2;
            existing.FeedbackWelcomeMessage3 = dto.FeedbackWelcomeMessage3;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = dto.ModifiedBy;
            existing.IsActive = dto.IsActive;

            var updated = await _unitOfWork.Operations.UpdateAsync(existing);
            if (updated)
            {
                return Ok(_mapper.Map<OperationsDTO>(existing));
            }
            return BadRequest("Unable to update Operations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating Operations {nameof(Put)}");
            throw;
        }
    }
}

