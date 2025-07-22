using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("RoomChkList")]
    public class RoomChkList
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? RID { get; set; }

        public string? RChkLstID { get; set; }

        public DateTime? ChkDate { get; set; }

        public int? CheckedBy { get; set; }

        public string? Reason { get; set; }

        public string? Comments { get; set; }
        
    }
}
