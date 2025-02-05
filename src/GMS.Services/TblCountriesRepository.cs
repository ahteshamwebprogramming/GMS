using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class TblCountriesRepository : DapperGenericRepository<TblCountries>, ITblCountriesRepository
{
    public TblCountriesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
