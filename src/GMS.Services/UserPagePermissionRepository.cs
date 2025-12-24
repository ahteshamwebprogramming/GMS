using Dapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Services.DBContext;

namespace GMS.Services
{
    public class UserPagePermissionRepository : DapperGenericRepository<UserPagePermission>, IUserPagePermissionRepository
    {
        public UserPagePermissionRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {
        }

        public async Task<List<UserPagePermission>> GetUserPageOverridesAsync(int userId)
        {
            // UserId in UserPagePermission table is INT (matches WorkerID as INT)
            string query = @"
                SELECT UserId, PageId, PermissionType, CreatedOn 
                FROM UserPagePermission 
                WHERE UserId = @UserId";
            
            var parameters = new { UserId = userId };
            var result = await GetTableData<UserPagePermission>(query, parameters);
            return result ?? new List<UserPagePermission>();
        }

        public async Task<List<UserPagePermission>> GetRolePagePermissionsAsync(int roleId)
        {
            // Get pages that the role has access to (from RoleMenuMapping)
            string query = @"
                SELECT DISTINCT 
                    @RoleId as UserId,  -- Dummy value, not used
                    rmm.MenuId as PageId,
                    'Allow' as PermissionType,
                    GETDATE() as CreatedOn
                FROM RoleMenuMapping rmm
                WHERE rmm.RoleId = @RoleId 
                    AND rmm.IsActive = 1
                    AND rmm.MenuId IS NOT NULL";
            
            var parameters = new { RoleId = roleId };
            var result = await GetTableData<UserPagePermission>(query, parameters);
            return result ?? new List<UserPagePermission>();
        }

        public async Task SaveUserPageOverridesAsync(int userId, List<UserPagePermission> overrides)
        {
            using var connection = DbConnection;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Delete existing overrides for this user
                string deleteQuery = "DELETE FROM UserPagePermission WHERE UserId = @UserId";
                await connection.ExecuteAsync(deleteQuery, new { UserId = userId }, transaction);
                
                // Insert new overrides
                if (overrides != null && overrides.Any())
                {
                    string insertQuery = @"
                        INSERT INTO UserPagePermission (UserId, PageId, PermissionType, CreatedOn)
                        VALUES (@UserId, @PageId, @PermissionType, @CreatedOn)";
                    
                    foreach (var overrideItem in overrides)
                    {
                        await connection.ExecuteAsync(insertQuery, new
                        {
                            UserId = userId,
                            PageId = overrideItem.PageId,
                            PermissionType = overrideItem.PermissionType,
                            CreatedOn = DateTime.Now
                        }, transaction);
                    }
                }
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

