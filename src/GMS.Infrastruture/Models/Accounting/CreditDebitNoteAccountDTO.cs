namespace GMS.Infrastructure.Models.Accounting
{
    public class CreditDebitNoteAccountDTO
    {
        public int Id { get; set; }

        public string? Code { get; set; }
        public double? Amount { get; set; }
        public DateTime? CodeValidity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? TransactionType { get; set; }
        public int? GuestId { get; set; }
        public int? SettlementId { get; set; }
        public double? UsedAmount { get; set; }
        public double? BalanceAmount { get; set; }
        public bool? IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool? IsRecovered { get; set; }
        public int? RecoveredBy { get; set; }
        public DateTime? RecoveredOn { get; set; }
    }
}
