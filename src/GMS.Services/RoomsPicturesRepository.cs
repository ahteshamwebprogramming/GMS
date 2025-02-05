using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class RoomsPicturesRepository : DapperGenericRepository<RoomsPictures>, IRoomsPicturesRepository
{
    public RoomsPicturesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}