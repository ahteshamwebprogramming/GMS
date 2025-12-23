using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class EmployeeTaskExecutionRepository : DapperGenericRepository<EmployeeTaskExecution>, IEmployeeTaskExecutionRepository
{
    public EmployeeTaskExecutionRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
    }
}








