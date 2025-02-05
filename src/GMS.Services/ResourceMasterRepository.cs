using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class ResourceMasterRepository : DapperGenericRepository<ResourceMaster>, IResourceMasterRepository
    {
        public ResourceMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
