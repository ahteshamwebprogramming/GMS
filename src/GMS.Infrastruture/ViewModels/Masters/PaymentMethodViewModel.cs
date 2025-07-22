using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters
{
    public class PaymentMethodViewModel
    {
        public PaymentMethodDTO? PaymentMethod { get; set; }
        public List<PaymentMethodDTO>? PaymentMethods { get; set; }
    }
}
