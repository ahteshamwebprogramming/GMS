using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("tblCity")]
public class tblCity
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public double? Id { get; set; }

    public string? City { get; set; }

    public string? Status { get; set; }
}
