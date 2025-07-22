using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("RoomAllocation")]
public class RoomAllocation
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Rnumber { get; set; }

    public int? Rtype { get; set; }

    public int? GuestId { get; set; }

    public DateTime? Fd { get; set; }

    public DateTime? Td { get; set; }

    public DateTime? AsigndDate { get; set; }

    public string? Remarks { get; set; }

    public int? IsActive { get; set; }

    public int? CreeatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? Shared { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? Reason { get; set; }

    public bool? NoShow { get; set; }
    public string? NoShowReason { get; set; }

}
