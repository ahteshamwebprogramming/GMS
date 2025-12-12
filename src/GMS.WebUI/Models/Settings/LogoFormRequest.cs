using Microsoft.AspNetCore.Http;

namespace GMS.WebUI.Models.Settings;

public class LogoFormRequest
{
    public int Id { get; set; }
    public IFormFile? LogoFile { get; set; }
    public string? LogoFilePath { get; set; }
}

