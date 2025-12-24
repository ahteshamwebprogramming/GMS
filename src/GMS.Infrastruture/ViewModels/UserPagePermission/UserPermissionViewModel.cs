using GMS.Infrastructure.ViewModels.UserPagePermission;

namespace GMS.Infrastructure.ViewModels.UserPagePermission
{
    public class UserPermissionViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<PagePermissionItem> Pages { get; set; } = new List<PagePermissionItem>();
    }
}

