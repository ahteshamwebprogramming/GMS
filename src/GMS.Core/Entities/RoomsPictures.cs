using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("RoomsPictures")]
public class RoomsPictures
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int RoomId { get; set; }

    public string? RoomPicturePath { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentExtension { get; set; }
    public bool? IsActive { get; set; }
}
