using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GMS.Core.Services;
using System.Security.Claims;

namespace GMS.WebUI.Filters
{
    /// <summary>
    /// Authorization filter that checks if the current user has permission to access a page
    /// Usage: [AuthorizePage("PageKey")] or [AuthorizePage(PageId = 123)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizePageAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string? PageKey { get; set; }
        public int? PageId { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userIdClaim = httpContext.User.FindFirst("WorkerId")?.Value ?? 
                             httpContext.User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Resolve IUserPermissionService from DI
            var userPermissionService = httpContext.RequestServices.GetService(typeof(IUserPermissionService)) as IUserPermissionService;
            
            if (userPermissionService == null)
            {
                // If service not available, allow access (fail open for backward compatibility)
                return;
            }

            int pageIdToCheck = 0;

            // Resolve PageId from PageKey or use provided PageId
            if (!string.IsNullOrEmpty(PageKey))
            {
                // You may need to implement a mapping from PageKey to PageId
                // For now, we'll try to get it from route or query
                pageIdToCheck = await ResolvePageIdFromKey(PageKey, httpContext, userPermissionService);
            }
            else if (PageId.HasValue)
            {
                pageIdToCheck = PageId.Value;
            }
            else
            {
                // Try to get pageId from route or query
                var routePageId = context.RouteData.Values["pageId"]?.ToString() ?? 
                                 context.HttpContext.Request.Query["pageId"].ToString();
                if (!string.IsNullOrEmpty(routePageId) && int.TryParse(routePageId, out int parsedPageId))
                {
                    pageIdToCheck = parsedPageId;
                }
                else
                {
                    // If we can't determine the page, allow access (fail open)
                    return;
                }
            }

            if (pageIdToCheck <= 0)
            {
                // Can't determine page ID, allow access (fail open)
                return;
            }

            bool canView = await userPermissionService.CanUserViewPageAsync(userId, pageIdToCheck);

            if (!canView)
            {
                context.Result = new ForbidResult();
            }
        }

        private async Task<int> ResolvePageIdFromKey(string pageKey, HttpContext httpContext, IUserPermissionService userPermissionService)
        {
            // This is a placeholder - you may need to implement a mapping service
            // For now, try to get it from MenuList by MenuLink or MenuName
            // You could cache this mapping or store it in a configuration
            
            // Example: Query MenuList table to find PageId by PageKey
            // This would require injecting IUnitOfWork or IMenuListRepository
            // For now, return 0 (which will fail open)
            
            return 0;
        }
    }
}

