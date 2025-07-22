using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Infrastructure.Models.EHRMS
{
    public class WorkerMasterDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? WorkerID { get; set; }
        public string? EMPID { get; set; }
        public string? FirstName { get; set; }
        public int? ManagerID { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public int? RoleID { get; set; }
        public string? WorkerName { get; set; }
        public int? GenderID { get; set; }
        public DateTime? DOB { get; set; }
        public int? DepartmentID { get; set; }
        public int? CurrentDesignation { get; set; }
    }
}
