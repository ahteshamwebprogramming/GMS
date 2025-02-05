using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("Amenities")]
public class Amenities
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? AmenityCategoryId { get; set; }
    public string? AmenityName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}
