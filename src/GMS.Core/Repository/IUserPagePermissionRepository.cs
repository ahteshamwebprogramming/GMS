using GMS.Core.Entities;

namespace GMS.Core.Repository;

public interface IUserPagePermissionRepository : IDapperRepository<UserPagePermission>
{
    Task<List<UserPagePermission>> GetUserPageOverridesAsync(int userId);
    Task<List<UserPagePermission>> GetRolePagePermissionsAsync(int roleId);
    Task SaveUserPageOverridesAsync(int userId, List<UserPagePermission> overrides);
}

