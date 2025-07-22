using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("Settlement")]
public class Settlement
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? GuestId { get; set; }
    public double? InvoicedAmount { get; set; }
    public double? PaymentCollected { get; set; }
    public double? Balance { get; set; }
    public double? Refund { get; set; }
    public double? CreditAmount { get; set; }
    public string? RefundRemarks { get; set; }
    public string? NoteNumber { get; set; }
    public DateTime? ValidTill { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public string? ApprovalComment { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public int? Status { get; set; }
    public string? InvoiceNumber { get; set; }
}
