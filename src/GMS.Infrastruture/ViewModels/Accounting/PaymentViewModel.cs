using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Guests;

namespace GMS.Infrastructure.ViewModels.Accounting
{
    public class PaymentViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<PaymentWithAttr>? PaymentsWithAttr { get; set; }
        public string? opt { get; set; }
    }
}
