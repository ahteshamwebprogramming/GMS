using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;
using Microsoft.AspNetCore.SignalR.Protocol;
namespace GMS.Services;

public class GMSFinalGuestRepository : DapperGenericRepository<GMSFinalGuest>, IGMSFinalGuestRepository
{
    public GMSFinalGuestRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
