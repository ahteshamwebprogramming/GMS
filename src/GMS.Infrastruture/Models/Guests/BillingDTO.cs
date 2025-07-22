namespace GMS.Infrastructure.Models.Guests;

public class BillingDTO
{
    public int Id { get; set; }
    public int? GuestId { get; set; }
    public string? ServiceType { get; set; }
    public string? ServiceName { get; set; }
    public int? ServiceId { get; set; }
    public int? Count { get; set; }
    public double? Price { get; set; }
    public double? Discount { get; set; }
    public string? DiscountCode { get; set; }
    public double? HSN_SAC { get; set; }
    public double? IGST { get; set; }
    public double? CGST { get; set; }
    public double? SGST { get; set; }
    public double? TotalAmount { get; set; }
    public bool? Confirmed { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public int? PostedToAudit { get; set; }
}
