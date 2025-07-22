using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.Models.Accounting
{
    public class InvoicingDTO : SettlementDTO
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDatetime { get; set; }
        public string? RNumber { get; set; }
        public string? GuestIds { get; set; }
        public string? UHIds { get; set; }
        public string? GuestNames { get; set; }
        public double? GrossAmount { get; set; }
        public double? Discount { get; set; }
        public double? SGST { get; set; }
        public double? IGST { get; set; }
        public double? CGST { get; set; }
        public double? AmountPayable { get; set; }
        public double? AmountReceived { get; set; }
        public double? Differences { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public int? Status { get; set; }
        public double? RoomCharges { get; set; }
        public double? TreatmentCharges { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NoOfNights { get; set; }
        public int? PackageId { get; set; }
        public string? Package { get; set; }
        public string? RoomType { get; set; }
    }
}
