using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class FeedbackRepository : DapperGenericRepository<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}