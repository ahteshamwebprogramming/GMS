using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Table("RoomHousekeepingAssignments")]
public class RoomHousekeepingAssignment
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AssignmentId { get; set; }

    public int RoomId { get; set; }

    public decimal WorkerId { get; set; }

    public DateTime WorkDate { get; set; }

    public string? ShiftCode { get; set; }

    public byte Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }
}

