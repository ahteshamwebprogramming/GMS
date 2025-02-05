namespace GMS.Infrastructure.Models.Rooms
{
    public class RoomsPicturesDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }

        public string? RoomPicturePath { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentExtension { get; set; }
        public bool? IsActive { get; set; }
    }
}
