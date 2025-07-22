using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("Payment")]
public class Payment
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? GuestId { get; set; }
    public int? PaymentMode { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime? DateOfPayment { get; set; }
    public double? Amount { get; set; }
    public string? Comments { get; set; }
    public double? AmountReceived { get; set; }
    public double? Differences { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovalComment { get; set; }

    public bool? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public int? Status { get; set; }
    public int? PostedToAudit { get; set; }

}
