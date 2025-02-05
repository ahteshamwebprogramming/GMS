using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("TblCountries")]
public class TblCountries
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public double? SrNo { get; set; }

    public string? Country { get; set; }
}
