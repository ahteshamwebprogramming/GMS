using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("GuaranteeCode")]
public class GuaranteeCode
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Code { get; set; }
}
