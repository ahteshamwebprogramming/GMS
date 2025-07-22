using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class RoleMenuMappingRepository : DapperGenericRepository<RoleMenuMapping>, IRoleMenuMappingRepository
    {
        public RoleMenuMappingRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
