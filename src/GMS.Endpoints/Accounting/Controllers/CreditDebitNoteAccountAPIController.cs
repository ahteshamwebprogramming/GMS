using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.ViewModels.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers;

[Route("api/CreditDebitNoteAccount")]
[ApiController]
public class CreditDebitNoteAccountAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreditDebitNoteAccountAPIController> _logger;
    private readonly IMapper _mapper;

    public CreditDebitNoteAccountAPIController(IUnitOfWork unitOfWork, ILogger<CreditDebitNoteAccountAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            string query = @"SELECT 
                            cdna.Id,
                            cdna.Code,
                            cdna.Amount,
                            cdna.CodeValidity,
                            cdna.CreatedDate,
                            cdna.CreatedBy,
                            cdna.IsActive,
                            cdna.ModifiedDate,
                            cdna.ModifiedBy,
                            cdna.TransactionType,
                            cdna.GuestId,
                            cdna.SettlementId,
                            cdna.UsedAmount,
                            cdna.BalanceAmount,
                            cdna.IsApproved,
                            cdna.ApprovedBy,
                            cdna.ApprovedOn,
                            cdna.IsRecovered,
                            cdna.RecoveredBy,
                            cdna.RecoveredOn,
                            md.CustomerName AS GuestName,
                            md.UHID
                            FROM CreditDebitNoteAccount cdna
                            LEFT JOIN MembersDetails md ON cdna.GuestId = md.Id
                            WHERE cdna.IsActive = 1
                            ORDER BY cdna.CreatedDate DESC";
            
            var result = await _unitOfWork.GenOperations.GetTableData<CreditDebitNoteAccountWithAttr>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving CreditDebitNoteAccount data in {nameof(GetAll)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving credit/debit note account data.");
        }
    }

    [HttpGet("Credit")]
    public async Task<IActionResult> GetCreditNotes()
    {
        try
        {
            string query = @"SELECT 
                            cdna.Id,
                            cdna.Code,
                            cdna.Amount,
                            cdna.CodeValidity,
                            cdna.CreatedDate,
                            cdna.CreatedBy,
                            cdna.IsActive,
                            cdna.ModifiedDate,
                            cdna.ModifiedBy,
                            cdna.TransactionType,
                            cdna.GuestId,
                            cdna.SettlementId,
                            cdna.UsedAmount,
                            cdna.BalanceAmount,
                            md.CustomerName AS GuestName,
                            md.UHID
                            FROM CreditDebitNoteAccount cdna
                            LEFT JOIN MembersDetails md ON cdna.GuestId = md.Id
                            WHERE cdna.IsActive = 1 AND cdna.TransactionType = 'Credit'
                            ORDER BY cdna.CreatedDate DESC";
            
            var result = await _unitOfWork.GenOperations.GetTableData<CreditDebitNoteAccountWithAttr>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving Credit Note Account data in {nameof(GetCreditNotes)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving credit note account data.");
        }
    }

    [HttpGet("Debit")]
    public async Task<IActionResult> GetDebitNotes()
    {
        try
        {
            string query = @"SELECT 
                            cdna.Id,
                            cdna.Code,
                            cdna.Amount,
                            cdna.CodeValidity,
                            cdna.CreatedDate,
                            cdna.CreatedBy,
                            cdna.IsActive,
                            cdna.ModifiedDate,
                            cdna.ModifiedBy,
                            cdna.TransactionType,
                            cdna.GuestId,
                            cdna.SettlementId,
                            cdna.UsedAmount,
                            cdna.BalanceAmount,
                            cdna.IsApproved,
                            cdna.ApprovedBy,
                            cdna.ApprovedOn,
                            cdna.IsRecovered,
                            cdna.RecoveredBy,
                            cdna.RecoveredOn,
                            md.CustomerName AS GuestName,
                            md.UHID
                            FROM CreditDebitNoteAccount cdna
                            LEFT JOIN MembersDetails md ON cdna.GuestId = md.Id
                            WHERE cdna.IsActive = 1 AND cdna.TransactionType = 'Debit'
                            ORDER BY cdna.CreatedDate DESC";
            
            var result = await _unitOfWork.GenOperations.GetTableData<CreditDebitNoteAccountWithAttr>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving Debit Note Account data in {nameof(GetDebitNotes)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving debit note account data.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            string query = @"SELECT 
                            cdna.Id,
                            cdna.Code,
                            cdna.Amount,
                            cdna.CodeValidity,
                            cdna.CreatedDate,
                            cdna.CreatedBy,
                            cdna.IsActive,
                            cdna.ModifiedDate,
                            cdna.ModifiedBy,
                            cdna.TransactionType,
                            cdna.GuestId,
                            cdna.SettlementId,
                            cdna.UsedAmount,
                            cdna.BalanceAmount,
                            md.CustomerName AS GuestName,
                            md.UHID
                            FROM CreditDebitNoteAccount cdna
                            LEFT JOIN MembersDetails md ON cdna.GuestId = md.Id
                            WHERE cdna.Id = @Id AND cdna.IsActive = 1";
            
            var param = new { @Id = id };
            var result = await _unitOfWork.GenOperations.GetEntityData<CreditDebitNoteAccountWithAttr>(query, param);
            
            if (result == null)
            {
                return NotFound($"Credit/Debit Note Account with ID {id} not found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving CreditDebitNoteAccount with ID {id} in {nameof(GetById)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving credit/debit note account data.");
        }
    }

    [HttpPut("{id}/CodeValidity")]
    public async Task<IActionResult> UpdateCodeValidity(int id, [FromBody] UpdateCodeValidityRequest request)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            string codeValidity = string.Empty;
            if (request != null)
            {
                codeValidity = request.CodeValidity ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(codeValidity))
            {
                return BadRequest("Code validity date is required");
            }

            if (!DateTime.TryParse(codeValidity, out var validityDate))
            {
                return BadRequest("Invalid date format");
            }

            // Get the existing record
            string query = @"SELECT * FROM CreditDebitNoteAccount WHERE Id = @Id AND IsActive = 1";
            var param = new { @Id = id };
            var entity = await _unitOfWork.GenOperations.GetEntityData<CreditDebitNoteAccount>(query, param);

            if (entity == null)
            {
                return NotFound($"Credit/Debit Note Account with ID {id} not found.");
            }

            // Check if the code is Active or Partially Used
            var amount = entity.Amount ?? 0;
            var balance = entity.BalanceAmount ?? 0;
            var isExpired = entity.CodeValidity.HasValue && entity.CodeValidity.Value.Date < DateTime.Today;

            if (isExpired)
            {
                return BadRequest("Cannot update validity for expired codes.");
            }

            if (balance == 0)
            {
                return BadRequest("Cannot update validity for used codes.");
            }

            // Update the CodeValidity
            entity.CodeValidity = validityDate;
            entity.ModifiedDate = DateTime.UtcNow;
            // Note: ModifiedBy should be set from the current user context if available

            var updated = await _unitOfWork.CreditDebitNoteAccount.UpdateAsync(entity);
            
            if (updated)
            {
                return Ok(new { message = "Code validity updated successfully." });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update code validity.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating CodeValidity for ID {id} in {nameof(UpdateCodeValidity)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating code validity.");
        }
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveDebitNote(int id, [FromBody] ApproveDebitNoteRequest request)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            // Get the existing record
            string query = @"SELECT * FROM CreditDebitNoteAccount WHERE Id = @Id AND IsActive = 1 AND TransactionType = 'Debit'";
            var param = new { @Id = id };
            var entity = await _unitOfWork.GenOperations.GetEntityData<CreditDebitNoteAccount>(query, param);

            if (entity == null)
            {
                return NotFound($"Debit Note with ID {id} not found.");
            }

            if (entity.IsApproved == true)
            {
                return BadRequest("Debit Note is already approved.");
            }

            // Update approval status
            entity.IsApproved = true;
            entity.ApprovedBy = request?.ApprovedBy;
            entity.ApprovedOn = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;

            var updated = await _unitOfWork.CreditDebitNoteAccount.UpdateAsync(entity);
            
            if (updated)
            {
                // Check if debit note is also recovered, then complete settlement
                if (entity.IsRecovered == true && entity.SettlementId.HasValue)
                {
                    await CompleteSettlementForDebitNote(entity.SettlementId.Value);
                }

                return Ok(new { success = true, message = "Debit Note approved successfully." });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to approve debit note.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving Debit Note with ID {id} in {nameof(ApproveDebitNote)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving debit note.");
        }
    }

    [HttpPut("{id}/mark-recovered")]
    public async Task<IActionResult> MarkDebitNoteAsRecovered(int id, [FromBody] MarkRecoveredRequest request)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            // Get the existing record
            string query = @"SELECT * FROM CreditDebitNoteAccount WHERE Id = @Id AND IsActive = 1 AND TransactionType = 'Debit'";
            var param = new { @Id = id };
            var entity = await _unitOfWork.GenOperations.GetEntityData<CreditDebitNoteAccount>(query, param);

            if (entity == null)
            {
                return NotFound($"Debit Note with ID {id} not found.");
            }

            if (entity.IsApproved != true)
            {
                return BadRequest("Debit Note must be approved before marking as recovered.");
            }

            if (entity.IsRecovered == true)
            {
                return BadRequest("Debit Note is already marked as recovered.");
            }

            // Update recovered status
            entity.IsRecovered = true;
            entity.RecoveredBy = request?.RecoveredBy;
            entity.RecoveredOn = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;

            var updated = await _unitOfWork.CreditDebitNoteAccount.UpdateAsync(entity);
            
            if (updated)
            {
                // Create payment entry for the debit note
                await CreatePaymentFromDebitNote(entity);

                // Check if settlement exists and complete it
                if (entity.SettlementId.HasValue)
                {
                    await CompleteSettlementForDebitNote(entity.SettlementId.Value);
                }

                return Ok(new { success = true, message = "Debit Note marked as recovered. Payment entry created and settlement completed." });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to mark debit note as recovered.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking Debit Note as recovered with ID {id} in {nameof(MarkDebitNoteAsRecovered)}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while marking debit note as recovered.");
        }
    }

    private async Task CreatePaymentFromDebitNote(CreditDebitNoteAccount debitNote)
    {
        try
        {
            // Get PaymentMethod ID for "DN"
            string paymentMethodQuery = @"SELECT Id FROM PaymentMethod WHERE PaymentMethodCode = 'DN' AND IsActive = 1";
            var paymentMethodId = await _unitOfWork.GenOperations.GetEntityData<int?>(paymentMethodQuery, null);

            if (!paymentMethodId.HasValue)
            {
                _logger.LogWarning("PaymentMethod with Code 'DN' not found. Cannot create payment entry for debit note.");
                return;
            }

            // Create payment entry
            var payment = new Payment
            {
                GuestId = debitNote.GuestId,
                Amount = debitNote.Amount,
                PaymentMode = paymentMethodId.Value,
                ReferenceNumber = debitNote.Code,
                DateOfPayment = DateTime.Now,
                Comments = $"Payment from Debit Note {debitNote.Code}",
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = debitNote.RecoveredBy ?? debitNote.CreatedBy
            };

            await _unitOfWork.Payment.AddAsync(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating payment from debit note {debitNote.Code}");
            throw;
        }
    }

    private async Task CompleteSettlementForDebitNote(int settlementId)
    {
        try
        {
            // Get the settlement
            string settlementQuery = @"SELECT * FROM Settlement WHERE Id = @SettlementId AND IsActive = 1";
            var settlement = await _unitOfWork.GenOperations.GetEntityData<Settlement>(settlementQuery, new { @SettlementId = settlementId });
            if (settlement == null)
            {
                return;
            }

            // Get all debit notes for this settlement
            string debitNoteQuery = @"SELECT * FROM CreditDebitNoteAccount 
                                    WHERE SettlementId = @SettlementId 
                                    AND TransactionType = 'Debit' 
                                    AND IsActive = 1";
            var debitNotes = await _unitOfWork.GenOperations.GetTableData<CreditDebitNoteAccount>(debitNoteQuery, new { @SettlementId = settlementId });

            // Check if all debit notes are approved and recovered
            bool allApprovedAndRecovered = debitNotes.All(dn => dn.IsApproved == true && dn.IsRecovered == true);

            if (allApprovedAndRecovered)
            {
                // Update settlement status to completed (Status = 2 means completed)
                settlement.Status = 2;
                settlement.ModifiedDate = DateTime.Now;
                await _unitOfWork.Settlement.UpdateAsync(settlement);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing settlement for SettlementId {settlementId}");
            throw;
        }
    }

    public class UpdateCodeValidityRequest
    {
        public string CodeValidity { get; set; } = string.Empty;
    }

    public class ApproveDebitNoteRequest
    {
        public int? ApprovedBy { get; set; }
    }

    public class MarkRecoveredRequest
    {
        public int? RecoveredBy { get; set; }
    }
}

