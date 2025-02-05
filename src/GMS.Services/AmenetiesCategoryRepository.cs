﻿using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class AmenetiesCategoryRepository : DapperGenericRepository<AmenetiesCategory>, IAmenetiesCategoryRepository
{
    public AmenetiesCategoryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}