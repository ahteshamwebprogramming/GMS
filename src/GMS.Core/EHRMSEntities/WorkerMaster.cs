using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.EHRMSEntities
{
    [Dapper.Contrib.Extensions.Table("WorkerMaster")]
    public class WorkerMaster
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? WorkerID { get; set; }
        public string? EMPID { get; set; }
        public string? FirstName { get; set; }
        public int? ManagerID { get; set; }        
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public int? RoleID { get; set; }

        [Computed]
        public string? WorkerName { get; set; }

        public int? GenderID { get; set; }
        public DateTime? DOB { get; set; }
        public int? DepartmentID { get; set; }
        public int? CurrentDesignation { get; set; }

    }
}
