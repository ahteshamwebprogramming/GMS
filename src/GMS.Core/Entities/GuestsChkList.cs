using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("GuestsChkList")]
public class GuestsChkList
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public int? GuestID { get; set; }
    public string? ChkListsID { get; set; }
    public DateTime? CrDate { get; set; }
    public string? CheklistOut { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }    
}
