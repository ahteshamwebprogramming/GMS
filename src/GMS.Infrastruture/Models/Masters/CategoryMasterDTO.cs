namespace GMS.Infrastructure.Models.Masters
{
    public class CategoryMasterDTO
    {
        public int Id { get; set; }

        public string? Category { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
