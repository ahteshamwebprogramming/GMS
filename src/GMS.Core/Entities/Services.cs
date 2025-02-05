using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("Services")]
public class Services
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Service { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateDate { get; set; }
}
