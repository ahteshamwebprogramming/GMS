using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GuestCancellationVerificationRepository : DapperGenericRepository<GuestCancellationVerification>, IGuestCancellationVerificationRepository
{
    public GuestCancellationVerificationRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
    }
}
