using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("MstCategory")]
public class MstCategory
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? ServiceId { get; set; }

    public string? Category { get; set; }

    public int? Status { get; set; }

    public int? NoOfNights { get; set; }
}
