namespace GMS.Infrastructure.Models.Rooms
{
    public class BulkCopyDTO
    {
        public int? PlanIdTo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? PlanIdFrom { get; set; }
    }
}
