using Accounting.Controllers;
using GMS.Infrastructure.ViewModels.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Accounting;

public class DebitNoteAccountController : Controller
{
    private readonly ILogger<DebitNoteAccountController> _logger;
    private readonly CreditDebitNoteAccountAPIController _creditDebitNoteAccountAPIController;

    public DebitNoteAccountController(
        ILogger<DebitNoteAccountController> logger,
        CreditDebitNoteAccountAPIController creditDebitNoteAccountAPIController)
    {
        _logger = logger;
        _creditDebitNoteAccountAPIController = creditDebitNoteAccountAPIController;
    }

    public IActionResult List()
    {
        ViewData["Title"] = "Debit Note Account";
        return View();
    }

    public async Task<IActionResult> ListPartialView(string status = "All")
    {
        var viewModel = new CreditDebitNoteAccountViewModel();

        try
        {
            var result = await _creditDebitNoteAccountAPIController.GetDebitNotes();
            if (result != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode == 200)
            {
                var allItems = (List<CreditDebitNoteAccountWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
                
                if (allItems != null && !string.IsNullOrEmpty(status) && status != "All")
                {
                    var filteredItems = FilterByStatus(allItems, status);
                    viewModel.CreditDebitNoteAccountsWithAttr = filteredItems;
                }
                else
                {
                    viewModel.CreditDebitNoteAccountsWithAttr = allItems;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Debit Note Account list");
        }

        return PartialView("_list/_debitNoteAccount", viewModel);
    }

    private List<CreditDebitNoteAccountWithAttr> FilterByStatus(List<CreditDebitNoteAccountWithAttr> items, string status)
    {
        var filtered = new List<CreditDebitNoteAccountWithAttr>();

        foreach (var item in items)
        {
            var isApproved = item.IsApproved ?? false;
            var isRecovered = item.IsRecovered ?? false;

            if (status == "ApprovalPending" && !isApproved)
            {
                // Not approved yet
                filtered.Add(item);
            }
            else if (status == "NotRecovered" && isApproved && !isRecovered)
            {
                // Approved but not recovered
                filtered.Add(item);
            }
            else if (status == "Recovered" && isRecovered)
            {
                // All that are recovered
                filtered.Add(item);
            }
            // "All" status is handled in the calling method by not filtering
        }

        return filtered;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCodeValidity([FromBody] UpdateCodeValidityRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return BadRequest(new { success = false, message = "Invalid request." });
        }

        if (string.IsNullOrWhiteSpace(request.CodeValidity))
        {
            return BadRequest(new { success = false, message = "Validity date is required." });
        }

        try
        {
            // Create instance of API controller's nested UpdateCodeValidityRequest class using reflection
            var apiRequestType = typeof(CreditDebitNoteAccountAPIController).GetNestedType("UpdateCodeValidityRequest", System.Reflection.BindingFlags.Public);
            if (apiRequestType == null)
            {
                return StatusCode(500, new { success = false, message = "Unable to create request object." });
            }
            var apiRequest = System.Activator.CreateInstance(apiRequestType);
            if (apiRequest != null)
            {
                var property = apiRequestType.GetProperty("CodeValidity");
                if (property != null)
                {
                    property.SetValue(apiRequest, request.CodeValidity);
                }
            }
            
            // Use reflection to call the method with the dynamically created object
            var method = typeof(CreditDebitNoteAccountAPIController).GetMethod("UpdateCodeValidity");
            if (method == null)
            {
                return StatusCode(500, new { success = false, message = "Unable to find update method." });
            }
            
            var task = (Task<IActionResult>)method.Invoke(_creditDebitNoteAccountAPIController, new object[] { request.Id, apiRequest });
            var result = await task;
            
            if (result is OkObjectResult okResult)
            {
                return Ok(new { success = true, message = "Code validity updated successfully." });
            }
            else if (result is BadRequestObjectResult badRequest)
            {
                return BadRequest(new { success = false, message = badRequest.Value?.ToString() ?? "Failed to update validity." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating validity." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating code validity for ID {Id}", request.Id);
            return StatusCode(500, new { success = false, message = "An error occurred while updating validity." });
        }
    }

    public class UpdateCodeValidityRequest
    {
        public int Id { get; set; }
        public string CodeValidity { get; set; } = string.Empty;
    }
}

