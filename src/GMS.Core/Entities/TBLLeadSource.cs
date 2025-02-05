using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("LeadSource")]
public class TBLLeadSource
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? LeadSource { get; set; }

    public int? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
