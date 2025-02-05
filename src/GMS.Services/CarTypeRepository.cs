﻿using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class CarTypeRepository : DapperGenericRepository<CarType>, ICarTypeRepository
{
    public CarTypeRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
