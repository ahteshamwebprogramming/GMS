using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("TblCheckLists")]
public class TblCheckLists
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public string? Chklist { get; set; }

    public int? Type { get; set; }
    public int? ChkIn { get; set; }
    public int? ChkOut { get; set; }
    public bool IsMandatory { get; set; }
    public string? ChecklistType { get; set; }
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
    public int? Score { get; set; }
}
