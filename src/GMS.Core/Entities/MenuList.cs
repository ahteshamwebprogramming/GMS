using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("MenuList")]
public class MenuList
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? MenuParentId { get; set; }
    public string? MenuName { get; set; }
    public string? MenuLink { get; set; }
    public string? MenuIcon { get; set; }
    public bool? IsActive { get; set; }
    public bool? SelfMenu { get; set; }
    public bool? DefaultMenu { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public int? SNo { get; set; }
}
