namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class RoomsWithAttribute
    {
        public int Id { get; set; }

        public string? Rnumber { get; set; }

        public string? Rtype { get; set; }

        public int? Status { get; set; }
        public bool CleanStatus { get; set; }

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

        public int? AttendedBy { get; set; }
        public string? AttendedByName { get; set; }
        public DateTime? AttendedDate{ get; set; }
        public string? Reason{ get; set; }
        public string? Comments{ get; set; }
    }
    
}
