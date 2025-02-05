using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("RoomLock")]
    public class RoomLock
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Rtype { get; set; }

        public string? Rooms { get; set; }

        public string? Fd { get; set; }

        public string? Ed { get; set; }

        public int? Status { get; set; }

        public DateTime? CrDate { get; set; }

        public string? Remarks { get; set; }
        public string? Type { get; set; }
    }
}
