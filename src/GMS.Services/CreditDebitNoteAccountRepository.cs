using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services;

public class CreditDebitNoteAccountRepository : DapperGenericRepository<CreditDebitNoteAccount>, ICreditDebitNoteAccountRepository
{
    public CreditDebitNoteAccountRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}