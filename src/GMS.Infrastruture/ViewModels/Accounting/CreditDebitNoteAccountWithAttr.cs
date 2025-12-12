using GMS.Infrastructure.Models.Accounting;

namespace GMS.Infrastructure.ViewModels.Accounting
{
    public class CreditDebitNoteAccountWithAttr : CreditDebitNoteAccountDTO
    {
        public string? GuestName { get; set; }
        public string? UHID { get; set; }
        // Approval and Recovery fields are inherited from CreditDebitNoteAccountDTO
    }
}

