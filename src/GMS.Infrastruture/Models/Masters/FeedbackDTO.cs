namespace GMS.Infrastructure.Models.Masters
{
    public class FeedbackDTO
    {
        public int Id { get; set; }

        public string? Question { get; set; }
        public string? Ansuar { get; set; }
        public string? OpenText1 { get; set; }
        public string? OpenText2 { get; set; }
        public string? ClientId { get; set; }
        public DateTime? CreationDate { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
