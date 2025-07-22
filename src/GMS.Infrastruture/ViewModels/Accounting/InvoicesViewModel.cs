using GMS.Infrastructure.Models.Accounting;

namespace GMS.Infrastructure.ViewModels.Accounting
{
    public class InvoicesViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<InvoicingDTO>? Invoicing { get; set; }
    }
}
