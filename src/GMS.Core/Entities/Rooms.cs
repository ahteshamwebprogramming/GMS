using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("Rooms")]
    public class Rooms
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Rnumber { get; set; }

        public string? Rtype { get; set; }

        public int? Status { get; set; }

        public int? RtypeId { get; set; }

        public int? IsChecked { get; set; }

        public string? Rsize { get; set; }

        public string? BedSize { get; set; }

        public string? Remarks { get; set; }

        public string? Img1 { get; set; }

        public string? Img2 { get; set; }

        public string? Img3 { get; set; }

        public string? Img4 { get; set; }

        public string? Img5 { get; set; }

        public string? DocName { get; set; }
    }
}
