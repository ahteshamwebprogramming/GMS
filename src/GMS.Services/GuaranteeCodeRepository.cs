using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GuaranteeCodeRepository : DapperGenericRepository<GuaranteeCode>, IGuaranteeCodeRepository
{
    public GuaranteeCodeRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
