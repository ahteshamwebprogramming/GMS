namespace GMS.WebUI.Models.Settings;

public class EmailFormRequest
{
    public int Id { get; set; }
    public string? SmtpServer { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public bool? SmtpEnableSsl { get; set; }
    public string? SmtpFromEmail { get; set; }
    public string? SmtpFromName { get; set; }
}

