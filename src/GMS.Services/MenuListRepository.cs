using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class MenuListRepository : DapperGenericRepository<MenuList>, IMenuListRepository
    {
        public MenuListRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
