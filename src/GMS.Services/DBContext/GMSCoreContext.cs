using Microsoft.EntityFrameworkCore;

namespace GMS.Services.DBContext;

public class GMSCoreContext : DbContext
{
    public GMSCoreContext()
    {
    }
    public GMSCoreContext(DbContextOptions<GMSCoreContext> options)
        : base(options)
    {
    }

}
