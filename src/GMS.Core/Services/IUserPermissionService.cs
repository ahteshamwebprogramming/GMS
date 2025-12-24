using GMS.Infrastructure.ViewModels.UserPagePermission;

namespace GMS.Core.Services
{
    public interface IUserPermissionService
    {
        Task<UserPermissionViewModel> GetUserPermissionViewModelAsync(int userId);
        Task<List<PagePermissionItem>> GetEffectivePermissionsAsync(int userId);
        Task<bool> CanUserViewPageAsync(int userId, int pageId);
        Task SaveUserPageOverridesAsync(int userId, List<GMS.Infrastructure.Models.UserPagePermission.UserPagePermissionDto> overrides);
    }
}

