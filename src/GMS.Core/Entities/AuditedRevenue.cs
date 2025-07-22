using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("AuditedRevenue")]
public class AuditedRevenue
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? GuestId { get; set; }

    public string? GroupId { get; set; }
    public string? RoomNumber { get; set; }
    public string? InvoiceNo { get; set; }
    public DateTime? Date { get; set; }
    public string? ChargesCategory { get; set; }
    public double? Charges { get; set; }
    public double? Taxes { get; set; }
    public double? TotalDueAmount { get; set; }
    public double? Payments { get; set; }
    public double? Balance { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public double? AdvancedPayment { get; set; }
}
