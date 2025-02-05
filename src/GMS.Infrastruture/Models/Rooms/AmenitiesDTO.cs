namespace GMS.Infrastructure.Models.Rooms
{
    public class AmenitiesDTO
    {
        public int Id { get; set; }

        public int? AmenityCategoryId { get; set; }
        public string? AmenityCategory { get; set; }
        public string? AmenityName { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
