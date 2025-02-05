namespace GMS.Infrastructure.Models.Masters
{
    public class LeadSourceDTO
    {
        public int Id { get; set; }

        public string? LeadSource { get; set; }

        public int? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
