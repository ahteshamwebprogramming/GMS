using Accounting.Controllers;
using GMS.Infrastructure.ViewModels.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Accounting;

public class CreditDebitNoteAccountController : Controller
{
    private readonly ILogger<CreditDebitNoteAccountController> _logger;
    private readonly CreditDebitNoteAccountAPIController _creditDebitNoteAccountAPIController;

    public CreditDebitNoteAccountController(
        ILogger<CreditDebitNoteAccountController> logger,
        CreditDebitNoteAccountAPIController creditDebitNoteAccountAPIController)
    {
        _logger = logger;
        _creditDebitNoteAccountAPIController = creditDebitNoteAccountAPIController;
    }

    public IActionResult List()
    {
        ViewData["Title"] = "Credit Note Account";
        return View();
    }

    public async Task<IActionResult> ListPartialView(string status = "All")
    {
        var viewModel = new CreditDebitNoteAccountViewModel();

        try
        {
            var result = await _creditDebitNoteAccountAPIController.GetCreditNotes();
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
            _logger.LogError(ex, "Error loading Credit Note Account list");
        }

        return PartialView("_list/_creditNoteAccount", viewModel);
    }

    private List<CreditDebitNoteAccountWithAttr> FilterByStatus(List<CreditDebitNoteAccountWithAttr> items, string status)
    {
        var filtered = new List<CreditDebitNoteAccountWithAttr>();
        var today = DateTime.Today;

        foreach (var item in items)
        {
            var amount = item.Amount ?? 0;
            var balance = item.BalanceAmount ?? 0;
            var isExpired = item.CodeValidity.HasValue && item.CodeValidity.Value.Date < today;

            if (status == "Expired" && isExpired)
            {
                filtered.Add(item);
            }
            else if (status == "Used" && balance == 0 && !isExpired)
            {
                filtered.Add(item);
            }
            else if (status == "PartiallyUsed" && balance > 0 && balance < amount && !isExpired)
            {
                filtered.Add(item);
            }
            else if (status == "Active" && balance == amount && amount > 0 && !isExpired)
            {
                filtered.Add(item);
            }
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

