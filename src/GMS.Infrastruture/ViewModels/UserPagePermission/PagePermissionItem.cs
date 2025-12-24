namespace GMS.Infrastructure.ViewModels.UserPagePermission
{
    public class PagePermissionItem
    {
        public int PageId { get; set; }
        public string PageName { get; set; } = string.Empty;
        public int? ParentPageId { get; set; }
        public bool RoleCanView { get; set; }
        public string? UserOverride { get; set; } // null | "Allow" | "Deny"
    }
}

