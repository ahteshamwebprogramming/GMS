namespace GMS.Infrastructure.Models.Guests
{
    public class SettlementDTO
    {
        public int Id { get; set; }
        public int? GuestId { get; set; }
        public int? GuestIdPaxSN1 { get; set; }
        public double? InvoicedAmount { get; set; }
        public double? PaymentCollected { get; set; }
        public double? Balance { get; set; }
        public double? Refund { get; set; }
        public double? CreditAmount { get; set; }
        public double? DebitAmount { get; set; }
        public string? RefundRemarks { get; set; }
        public string? NoteNumber { get; set; }
        public string? DebitNoteNumber { get; set; }
        public DateTime? ValidTill { get; set; }
        public DateTime? DebitNoteValidTill { get; set; }
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
        public bool? DebitNoteIsApproved { get; set; }
        public bool? DebitNoteIsRecovered { get; set; }
    }
}
