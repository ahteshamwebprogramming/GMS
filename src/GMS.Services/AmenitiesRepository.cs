using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class AmenitiesRepository : DapperGenericRepository<Amenities>, IAmenitiesRepository
{
    public AmenitiesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}