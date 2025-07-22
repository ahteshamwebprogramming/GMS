using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("FeedbackResults")]
public class FeedbackResults
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? FeedbackId { get; set; }
    public string? Ans { get; set; }
    public string? Opntxt1 { get; set; }
    public string? Opntxt2 { get; set; }
    public int? ClientId { get; set; }
    public DateTime? FeedbackDate { get; set; }
    public int? GuestId { get; set; }
    public int? Answer { get; set; }
    public string? FeedbackType { get; set; }
}
