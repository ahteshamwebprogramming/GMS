namespace GMS.Infrastructure.Models.Accounting
{
    public class GuestLedgerDTO
    {
        public int Id { get; set; }
        public string? GroupId { get; set; }
        public string? RoomNumber { get; set; }
        public double? TotalPayment { get; set; }
        public double? TotalCharges { get; set; }
        public double? TotalBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
