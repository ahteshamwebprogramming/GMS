# User-Wise Page Permissions Implementation Guide

## Overview
This implementation adds user-specific page permissions with role inheritance and user overrides to the GMS ASP.NET Core MVC application.

## Database Setup

### 1. Run SQL Script
Execute the SQL script located at:
```
Database/UserPagePermission_CreateTable.sql
```

This creates the `UserPagePermission` table with:
- `UserId` (INT, FK → WorkerMaster.WorkerID)
- `PageId` (INT, FK → MenuList.Id)
- `PermissionType` (VARCHAR(10)) - values: 'Allow', 'Deny'
- `CreatedOn` (DATETIME, default GETDATE())
- UNIQUE constraint on (UserId, PageId)

## Architecture Components

### 1. Entity Layer
- **UserPagePermission.cs** - Entity class for the database table

### 2. DTO/ViewModel Layer
- **UserPagePermissionDto.cs** - Data transfer object
- **UserPermissionViewModel.cs** - View model for the edit page
- **PagePermissionItem.cs** - Individual page permission item

### 3. Repository Layer
- **IUserPagePermissionRepository** - Repository interface
- **UserPagePermissionRepository** - Dapper-based repository implementation

### 4. Service Layer
- **IUserPermissionService** - Service interface
- **UserPermissionService** - Service implementation with permission resolution logic

### 5. Controller Layer
- **UserPermissionController** - MVC controller with Index, Edit, and Save actions

### 6. View Layer
- **Views/UserPermission/Index.cshtml** - User selection page
- **Views/UserPermission/Edit.cshtml** - Permission editing page

### 7. Security Layer
- **AuthorizePageAttribute** - Authorization filter for page-level security

## Permission Resolution Logic

The system uses the following priority:
1. **User Override = Deny** → User CANNOT view the page
2. **User Override = Allow** → User CAN view the page
3. **No User Override** → Use Role permission

## Usage Examples

### 1. Using the Authorization Filter

```csharp
[AuthorizePage(PageId = 123)]
public IActionResult MyPage()
{
    return View();
}

// Or using PageKey (requires implementation of PageKey mapping)
[AuthorizePage(PageKey = "MyPageKey")]
public IActionResult AnotherPage()
{
    return View();
}
```

### 2. Checking Permissions Programmatically

```csharp
public class MyController : Controller
{
    private readonly IUserPermissionService _userPermissionService;
    
    public MyController(IUserPermissionService userPermissionService)
    {
        _userPermissionService = userPermissionService;
    }
    
    public async Task<IActionResult> MyAction()
    {
        int userId = int.Parse(User.FindFirst("WorkerId")?.Value ?? "0");
        int pageId = 123;
        
        bool canView = await _userPermissionService.CanUserViewPageAsync(userId, pageId);
        
        if (!canView)
        {
            return Forbid();
        }
        
        return View();
    }
}
```

### 3. Getting Effective Permissions

```csharp
var effectivePermissions = await _userPermissionService.GetEffectivePermissionsAsync(userId);

foreach (var permission in effectivePermissions)
{
    bool canView = permission.UserOverride == "Allow" || 
                   (permission.UserOverride == null && permission.RoleCanView);
    
    if (canView)
    {
        // Show menu item
    }
}
```

## Menu Rendering

The menu rendering has been updated to use effective permissions. The `GetMenuDetails` method in `AccountsAPIController` now:
1. Checks if `UserId` is provided
2. If yes, uses `IUserPermissionService` to get effective permissions
3. Filters menu items based on effective permissions
4. Falls back to role-based permissions if service is not available

## Accessing User Permissions UI

1. Navigate to: `/UserPermission/Index`
2. Select a user from the dropdown
3. Click "Edit Permissions"
4. Modify permissions as needed:
   - Green badge = Inherited from role
   - Blue badge = User override Allow
   - Red badge = User override Deny
5. Click "Save Permissions"

## Important Notes

1. **Backward Compatibility**: The system maintains backward compatibility. If `IUserPermissionService` is not available or `UserId` is not provided, it falls back to role-based permissions.

2. **Performance**: Permission checks are optimized to fetch all permissions in a single query per user.

3. **Security**: The `AuthorizePageAttribute` filter checks permissions before executing controller actions.

4. **Session Management**: Menu list in session is updated during login to reflect effective permissions.

## Testing Checklist

- [ ] Run SQL script to create table
- [ ] Verify user can access `/UserPermission/Index`
- [ ] Verify user dropdown loads correctly
- [ ] Verify permission editing page loads
- [ ] Verify saving permissions works
- [ ] Verify menu rendering respects user overrides
- [ ] Verify role permissions still work
- [ ] Verify authorization filter works
- [ ] Verify login flow updates menu with effective permissions

## Troubleshooting

### Menu not showing user overrides
- Check that `UserId` is being passed to `GetMenuDetails`
- Verify `IUserPermissionService` is registered in `Program.cs`
- Check database for `UserPagePermission` records

### Authorization filter not working
- Ensure `IUserPermissionService` is registered in DI container
- Verify user claims contain `WorkerId` or `Id`
- Check that `PageId` or `PageKey` is correctly specified

### Permission save not working
- Check database connection
- Verify foreign key constraints (UserId exists in WorkerMaster, PageId exists in MenuList)
- Check server logs for errors

