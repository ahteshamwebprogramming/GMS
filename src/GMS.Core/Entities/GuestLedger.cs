using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("GuestLedger")]
    public class GuestLedger
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? GroupId { get; set; }
        public string? RoomNumber { get; set; }
        public double? TotalPayment { get; set; }
        public double? TotalCharges { get; set; }
        [Computed]
        public double? TotalBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
