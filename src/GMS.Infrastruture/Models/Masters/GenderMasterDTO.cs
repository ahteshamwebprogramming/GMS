using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Infrastructure.Models.Masters;

public class GenderMasterDTO
{
    public int Id { get; set; }

    public string? Gender { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
