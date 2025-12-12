using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.WebUI.Models.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GMS.WebUI.Controllers.Settings;

[Route("Settings/[controller]")]
public class OperationsController : Controller
{
    private readonly ILogger<OperationsController> _logger;
    private readonly OperationsAPIController _operationsAPIController;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public OperationsController(
        ILogger<OperationsController> logger,
        OperationsAPIController operationsAPIController,
        IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _operationsAPIController = operationsAPIController;
        _webHostEnvironment = webHostEnvironment;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var model = await GetOperationsAsync() ?? new OperationsDTO 
        { 
            IsActive = true,
            SmtpPort = 587,
            SmtpEnableSsl = true
        };
        return View("~/Views/Operations/Index.cshtml", model);
    }

    [HttpPost]
    [Route("SaveLogo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveLogo([FromForm] LogoFormRequest inputVM)
    {
        if (inputVM == null)
        {
            return BadRequest("Logo information is required");
        }

        // Get existing operations to preserve email and feedback settings
        var existingDto = await GetOperationsAsync();
        var dto = existingDto ?? new OperationsDTO 
        { 
            IsActive = true,
            SmtpPort = 587,
            SmtpEnableSsl = true
        };

        // Handle logo file upload
        if (inputVM.LogoFile != null && inputVM.LogoFile.Length > 0)
        {
            var logoPath = await SaveLogoFileAsync(inputVM.LogoFile);
            if (logoPath == null)
            {
                return BadRequest("Invalid logo file type. Please upload JPG, JPEG, PNG, GIF, or WEBP images.");
            }
            dto.LogoFilePath = logoPath;
        }
        else if (!string.IsNullOrWhiteSpace(inputVM.LogoFilePath))
        {
            // Keep existing logo if no new file uploaded
            dto.LogoFilePath = inputVM.LogoFilePath;
        }
        else if (existingDto != null)
        {
            // Preserve existing logo
            dto.LogoFilePath = existingDto.LogoFilePath;
        }

        dto.Id = inputVM.Id > 0 ? inputVM.Id : (existingDto?.Id ?? 0);
        
        // Preserve existing email and feedback settings
        if (existingDto != null)
        {
            dto.SmtpServer = existingDto.SmtpServer;
            dto.SmtpPort = existingDto.SmtpPort;
            dto.SmtpUsername = existingDto.SmtpUsername;
            dto.SmtpPassword = existingDto.SmtpPassword;
            dto.SmtpEnableSsl = existingDto.SmtpEnableSsl;
            dto.SmtpFromEmail = existingDto.SmtpFromEmail;
            dto.SmtpFromName = existingDto.SmtpFromName;
            dto.FeedbackWelcomeTitle = existingDto.FeedbackWelcomeTitle;
            dto.FeedbackWelcomeMessage1 = existingDto.FeedbackWelcomeMessage1;
            dto.FeedbackWelcomeMessage2 = existingDto.FeedbackWelcomeMessage2;
            dto.FeedbackWelcomeMessage3 = existingDto.FeedbackWelcomeMessage3;
        }

        return await _operationsAPIController.Post(dto);
    }

    [HttpPost]
    [Route("SaveEmail")]
    public async Task<IActionResult> SaveEmail([FromBody] EmailFormRequest inputVM)
    {
        if (inputVM == null)
        {
            return BadRequest("Email settings are required");
        }

        // Get existing operations to preserve logo and feedback settings
        var existingDto = await GetOperationsAsync();
        var dto = existingDto ?? new OperationsDTO 
        { 
            IsActive = true,
            SmtpPort = 587,
            SmtpEnableSsl = true
        };

        // Update only email fields
        dto.Id = inputVM.Id > 0 ? inputVM.Id : (existingDto?.Id ?? 0);
        dto.SmtpServer = inputVM.SmtpServer;
        dto.SmtpPort = inputVM.SmtpPort;
        dto.SmtpUsername = inputVM.SmtpUsername;
        dto.SmtpPassword = inputVM.SmtpPassword;
        dto.SmtpEnableSsl = inputVM.SmtpEnableSsl;
        dto.SmtpFromEmail = inputVM.SmtpFromEmail;
        dto.SmtpFromName = inputVM.SmtpFromName;
        
        // Preserve existing logo and feedback settings
        if (existingDto != null)
        {
            dto.LogoFilePath = existingDto.LogoFilePath;
            dto.FeedbackWelcomeTitle = existingDto.FeedbackWelcomeTitle;
            dto.FeedbackWelcomeMessage1 = existingDto.FeedbackWelcomeMessage1;
            dto.FeedbackWelcomeMessage2 = existingDto.FeedbackWelcomeMessage2;
            dto.FeedbackWelcomeMessage3 = existingDto.FeedbackWelcomeMessage3;
        }

        return await _operationsAPIController.Post(dto);
    }

    [HttpPost]
    [Route("SaveFeedback")]
    public async Task<IActionResult> SaveFeedback([FromBody] FeedbackFormRequest inputVM)
    {
        try
        {
            if (inputVM == null)
            {
                return BadRequest("Feedback content is required");
            }

            // Get existing operations to preserve logo and email settings
            var existingDto = await GetOperationsAsync();
            var dto = existingDto ?? new OperationsDTO 
            { 
                IsActive = true,
                SmtpPort = 587,
                SmtpEnableSsl = true
            };

            // Update only feedback fields
            dto.Id = inputVM.Id > 0 ? inputVM.Id : (existingDto?.Id ?? 0);
            dto.FeedbackWelcomeTitle = inputVM.FeedbackWelcomeTitle ?? string.Empty;
            dto.FeedbackWelcomeMessage1 = inputVM.FeedbackWelcomeMessage1 ?? string.Empty;
            dto.FeedbackWelcomeMessage2 = inputVM.FeedbackWelcomeMessage2 ?? string.Empty;
            dto.FeedbackWelcomeMessage3 = inputVM.FeedbackWelcomeMessage3 ?? string.Empty;
            
            // Preserve existing logo and email settings
            if (existingDto != null)
            {
                dto.LogoFilePath = existingDto.LogoFilePath;
                dto.SmtpServer = existingDto.SmtpServer;
                dto.SmtpPort = existingDto.SmtpPort;
                dto.SmtpUsername = existingDto.SmtpUsername;
                dto.SmtpPassword = existingDto.SmtpPassword;
                dto.SmtpEnableSsl = existingDto.SmtpEnableSsl;
                dto.SmtpFromEmail = existingDto.SmtpFromEmail;
                dto.SmtpFromName = existingDto.SmtpFromName;
            }

            var result = await _operationsAPIController.Post(dto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving feedback content");
            return StatusCode(500, $"Error saving feedback content: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("GetOperations")]
    public async Task<IActionResult> GetOperations()
    {
        var dto = await GetOperationsAsync();
        return Json(dto ?? new OperationsDTO 
        { 
            IsActive = true,
            SmtpPort = 587,
            SmtpEnableSsl = true
        });
    }

    private async Task<OperationsDTO?> GetOperationsAsync()
    {
        var response = await _operationsAPIController.Get();

        if (response is ObjectResult objectResult &&
            (objectResult.StatusCode is null || (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)))
        {
            return objectResult.Value as OperationsDTO;
        }

        return null;
    }

    private async Task<string?> SaveLogoFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "operations");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"logo_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/operations/{fileName}".Replace("\\", "/");
    }
}

