using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class PaymentRepository : DapperGenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
