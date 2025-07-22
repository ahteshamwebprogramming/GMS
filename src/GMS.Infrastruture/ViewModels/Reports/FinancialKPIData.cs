namespace GMS.Infrastructure.ViewModels.Reports
{
    public class FinancialKPIData
    {
        public DateTime? TheDate { get; set; }
        public double? SalesRevenue { get; set; }
        public double? PaymentCollected { get; set; }
        public double? PaymentDue { get; set; }
        public double? AverageRevenueLoss { get; set; }
        public double? FirstSale { get; set; }
        public double? LastSale { get; set; }
        public double? HighestSale { get; set; }
        public double? LowestSale { get; set; }
    }
}
