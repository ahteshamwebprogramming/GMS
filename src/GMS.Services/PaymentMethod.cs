using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class PaymentMethodRepository : DapperGenericRepository<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}