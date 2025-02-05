using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("RoomAmeneties")]
    public class RoomAmeneties
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? RoomNumber { get; set; }
        public int? AmenityId { get; set; }

    }
}
