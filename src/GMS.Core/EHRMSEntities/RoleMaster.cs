using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.EHRMSEntities
{
    [Dapper.Contrib.Extensions.Table("RoleMaster")]
    public class RoleMaster
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public char? isDefault { get; set; }
        public char? isActive { get; set; }
    }
}
