using Dapper;
using GMS.Core.Repository;
using GMS.Core.Services;
using GMS.Infrastructure.Models.RoleMenuMapping;
using GMS.Infrastructure.Models.UserPagePermission;
using GMS.Infrastructure.ViewModels.UserPagePermission;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPagePermissionRepository _userPagePermissionRepository;

        public UserPermissionService(IUnitOfWork unitOfWork, IUserPagePermissionRepository userPagePermissionRepository)
        {
            _unitOfWork = unitOfWork;
            _userPagePermissionRepository = userPagePermissionRepository;
        }

        public async Task<UserPermissionViewModel> GetUserPermissionViewModelAsync(int userId)
        {
            // Get user details with role
            // Note: WorkerID is decimal in database, so we cast to int for comparison
            string userQuery = @"
                SELECT wm.WorkerID as UserId, 
                       wm.WorkerName as UserName,
                       wm.RoleID as RoleId,
                       rm.RoleName
                FROM EHRMS.dbo.WorkerMaster wm
                LEFT JOIN EHRMS.dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                WHERE CAST(wm.WorkerID AS INT) = @UserId";
            
            var userParams = new { UserId = userId };
            
            var userData = await _unitOfWork.EHRMSLogin.GetEntityData<dynamic>(userQuery, userParams);
            
            if (userData == null)
            {
                // Try alternative query with decimal comparison
                string altQuery = @"
                    SELECT TOP 1 wm.WorkerID as UserId, 
                           wm.WorkerName as UserName,
                           wm.RoleID as RoleId,
                           rm.RoleName
                    FROM EHRMS.dbo.WorkerMaster wm
                    LEFT JOIN EHRMS.dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                    WHERE wm.WorkerID = @UserIdDecimal";
                
                var altParams = new { UserIdDecimal = (decimal)userId };
                userData = await _unitOfWork.EHRMSLogin.GetEntityData<dynamic>(altQuery, altParams);
                
                if (userData == null)
                {
                    throw new Exception($"User with ID {userId} not found in WorkerMaster table. Please verify the user exists.");
                }
            }
            
            int roleId = userData.RoleId != null ? Convert.ToInt32(userData.RoleId) : 0;
            // WorkerID is decimal, convert to int for UserId
            decimal? workerIdDecimal = userData.UserId != null ? Convert.ToDecimal(userData.UserId) : null;
            int actualUserId = workerIdDecimal.HasValue ? Convert.ToInt32(workerIdDecimal.Value) : userId;
            string userName = userData.UserName?.ToString() ?? string.Empty;
            string roleName = userData.RoleName?.ToString() ?? string.Empty;
            
            // Get all pages from MenuList
            string pagesQuery = "SELECT Id, MenuParentId, MenuName FROM MenuList WHERE IsActive = 1 ORDER BY SNo";
            var allPages = await _unitOfWork.MenuList.GetTableData<MenuListDTO>(pagesQuery);
            
            // Get role permissions
            var rolePermissions = await _userPagePermissionRepository.GetRolePagePermissionsAsync(roleId);
            var rolePageIds = rolePermissions.Select(p => p.PageId).ToHashSet();
            
            // Get user overrides
            var userOverrides = await _userPagePermissionRepository.GetUserPageOverridesAsync(userId);
            var userOverrideDict = userOverrides.ToDictionary(u => u.PageId, u => u.PermissionType);
            
            // Build page permission items
            var pageItems = allPages.Select(page => new PagePermissionItem
            {
                PageId = page.Id,
                PageName = page.MenuName ?? string.Empty,
                ParentPageId = page.MenuParentId,
                RoleCanView = rolePageIds.Contains(page.Id),
                UserOverride = userOverrideDict.ContainsKey(page.Id) ? userOverrideDict[page.Id] : null
            }).ToList();
            
            return new UserPermissionViewModel
            {
                UserId = actualUserId,
                UserName = userName,
                RoleId = roleId,
                RoleName = roleName,
                Pages = pageItems
            };
        }

        public async Task<List<PagePermissionItem>> GetEffectivePermissionsAsync(int userId)
        {
            // Get user with role
            // Note: WorkerID is decimal in database, so we cast to int for comparison
            string userQuery = @"
                SELECT wm.WorkerID as UserId, wm.RoleID as RoleId
                FROM EHRMS.dbo.WorkerMaster wm
                WHERE CAST(wm.WorkerID AS INT) = @UserId";
            
            var userParams = new { UserId = userId };
            var userData = await _unitOfWork.EHRMSLogin.GetEntityData<dynamic>(userQuery, userParams);
            
            if (userData == null)
            {
                return new List<PagePermissionItem>();
            }

            int roleId = userData.RoleId != null ? Convert.ToInt32(userData.RoleId) : 0;
            
            // Get all pages
            string pagesQuery = "SELECT Id, MenuParentId, MenuName FROM MenuList WHERE IsActive = 1";
            var allPages = await _unitOfWork.MenuList.GetTableData<MenuListDTO>(pagesQuery);
            
            // Get role permissions
            var rolePermissions = await _userPagePermissionRepository.GetRolePagePermissionsAsync(roleId);
            var rolePageIds = rolePermissions.Select(p => p.PageId).ToHashSet();
            
            // Get user overrides
            var userOverrides = await _userPagePermissionRepository.GetUserPageOverridesAsync(userId);
            var userOverrideDict = userOverrides.ToDictionary(u => u.PageId, u => u.PermissionType);
            
            // Calculate effective permissions
            var effectivePermissions = allPages.Select(page =>
            {
                bool roleCanView = rolePageIds.Contains(page.Id);
                bool canView = roleCanView;
                
                if (userOverrideDict.ContainsKey(page.Id))
                {
                    canView = userOverrideDict[page.Id] == "Allow";
                }
                
                return new PagePermissionItem
                {
                    PageId = page.Id,
                    PageName = page.MenuName ?? string.Empty,
                    ParentPageId = page.MenuParentId,
                    RoleCanView = roleCanView,
                    UserOverride = userOverrideDict.ContainsKey(page.Id) ? userOverrideDict[page.Id] : null
                };
            }).ToList();
            
            return effectivePermissions;
        }

        public async Task<bool> CanUserViewPageAsync(int userId, int pageId)
        {
            var effectivePermissions = await GetEffectivePermissionsAsync(userId);
            var pagePermission = effectivePermissions.FirstOrDefault(p => p.PageId == pageId);
            
            if (pagePermission == null)
            {
                return false;
            }
            
            // If user override exists, use it; otherwise use role permission
            if (pagePermission.UserOverride != null)
            {
                return pagePermission.UserOverride == "Allow";
            }
            
            return pagePermission.RoleCanView;
        }

        public async Task SaveUserPageOverridesAsync(int userId, List<UserPagePermissionDto> overrides)
        {
            var overrideEntities = overrides.Select(o => new GMS.Core.Entities.UserPagePermission
            {
                UserId = userId,
                PageId = o.PageId,
                PermissionType = o.PermissionType,
                CreatedOn = DateTime.Now
            }).ToList();
            
            await _userPagePermissionRepository.SaveUserPageOverridesAsync(userId, overrideEntities);
        }
    }
}

