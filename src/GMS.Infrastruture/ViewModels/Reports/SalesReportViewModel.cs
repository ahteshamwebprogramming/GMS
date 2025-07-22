using GMS.Infrastructure.Models.Guests;

namespace GMS.Infrastructure.ViewModels.Reports
{
    public class SalesReportViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<SettlementDTO>? Settlements { get; set; }
        public List<AuditedRevenueDTO>? AuditedRevenues { get; set; }
    }
}
