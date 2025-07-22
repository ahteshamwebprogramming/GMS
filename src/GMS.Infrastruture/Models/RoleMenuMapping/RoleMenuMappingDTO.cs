namespace GMS.Infrastructure.Models.RoleMenuMapping
{
    public class RoleMenuMappingDTO
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? MenuId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
