using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("UserPagePermission")]
public class UserPagePermission
{
    [Dapper.Contrib.Extensions.Key]
    public int UserId { get; set; }
    
    [Dapper.Contrib.Extensions.Key]
    public int PageId { get; set; }
    
    public string PermissionType { get; set; } = string.Empty; // 'Allow' or 'Deny'
    
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}

