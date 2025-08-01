﻿namespace GMS.Infrastructure.Models.Accounting
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
    }
}
