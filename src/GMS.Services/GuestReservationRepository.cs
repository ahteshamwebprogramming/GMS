using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class GuestReservationRepository : DapperGenericRepository<GuestReservation>, IGuestReservationRepository
{
    public GuestReservationRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
