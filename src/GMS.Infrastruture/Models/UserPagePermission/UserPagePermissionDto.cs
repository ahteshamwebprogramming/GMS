namespace GMS.Infrastructure.Models.UserPagePermission
{
    public class UserPagePermissionDto
    {
        public int UserId { get; set; }
        public int PageId { get; set; }
        public string PermissionType { get; set; } = string.Empty; // 'Allow' or 'Deny'
    }
}

