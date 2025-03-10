﻿using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class RoomLockRepository : DapperGenericRepository<RoomLock>, IRoomLockRepository
{
    public RoomLockRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
