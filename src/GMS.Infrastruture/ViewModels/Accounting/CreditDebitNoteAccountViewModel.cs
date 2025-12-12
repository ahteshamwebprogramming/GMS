using GMS.Infrastructure.Models.Accounting;

namespace GMS.Infrastructure.ViewModels.Accounting
{
    public class CreditDebitNoteAccountViewModel
    {
        public CreditDebitNoteAccountDTO? CreditDebitNoteAccount { get; set; }
        public List<CreditDebitNoteAccountDTO>? CreditDebitNoteAccountsList { get; set; }
        public List<CreditDebitNoteAccountWithAttr>? CreditDebitNoteAccountsWithAttr { get; set; }
    }
}

