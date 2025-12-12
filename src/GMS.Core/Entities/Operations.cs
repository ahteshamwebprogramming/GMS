using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("Operations")]
public class Operations
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? LogoFilePath { get; set; }
    
    // Email Settings
    public string? SmtpServer { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public bool? SmtpEnableSsl { get; set; }
    public string? SmtpFromEmail { get; set; }
    public string? SmtpFromName { get; set; }
    
    // Feedback Module Text Content
    public string? FeedbackWelcomeTitle { get; set; }
    public string? FeedbackWelcomeMessage1 { get; set; }
    public string? FeedbackWelcomeMessage2 { get; set; }
    public string? FeedbackWelcomeMessage3 { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}

