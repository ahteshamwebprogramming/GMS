namespace GMS.Infrastructure.Models.Rooms
{
    public class RoomsDTO
    {
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
        public bool RoomClean { get; set; }
        public bool EnableSelection { get; set; }
    }
}
