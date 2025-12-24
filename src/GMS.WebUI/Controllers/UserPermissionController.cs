using GMS.Core.Repository;
using GMS.Core.Services;
using GMS.Infrastructure.Models.UserPagePermission;
using GMS.Infrastructure.ViewModels.UserPagePermission;
using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers
{
    public class UserPermissionController : Controller
    {
        private readonly IUserPermissionService _userPermissionService;
        private readonly ILogger<UserPermissionController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserPermissionController(
            IUserPermissionService userPermissionService,
            ILogger<UserPermissionController> logger,
            IUnitOfWork unitOfWork)
        {
            _userPermissionService = userPermissionService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all users for dropdown
                var usersQuery = @"
                    SELECT wm.WorkerID as UserId, 
                           wm.WorkerName as UserName,
                           wm.RoleID as RoleId,
                           rm.RoleName
                    FROM EHRMS.dbo.WorkerMaster wm
                    LEFT JOIN EHRMS.dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                    WHERE wm.WorkerID IS NOT NULL
                    ORDER BY wm.WorkerName";
                
                // We'll need to get users through the unit of work
                // For now, return view with empty model - users will be loaded via AJAX or we'll inject UnitOfWork
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading UserPermission Index");
                return View("Error");
            }
        }

        [HttpGet]
        [Route("UserPermission/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation($"Edit action called - Route: {Request.Path}, QueryString: {Request.QueryString}, id parameter: {id}");
                
                // Also check route values
                foreach (var routeValue in RouteData.Values)
                {
                    _logger.LogInformation($"RouteData[{routeValue.Key}] = {routeValue.Value}");
                }
                
                if (id <= 0)
                {
                    _logger.LogWarning($"Invalid user ID received: {id}. Request Path: {Request.Path}");
                    TempData["Error"] = $"Invalid user ID: {id}. Please ensure you select a valid user.";
                    return RedirectToAction("Index");
                }

                _logger.LogInformation($"Attempting to load user permissions for userId: {id}");
                var viewModel = await _userPermissionService.GetUserPermissionViewModelAsync(id);
                _logger.LogInformation($"Successfully loaded view model for userId: {id}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user permissions. Exception: {ex.Message}. StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Error loading user permissions: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserPermissionSaveModel model)
        {
            try
            {
                if (model == null || model.UserId <= 0)
                {
                    return BadRequest("Invalid data");
                }

                await _userPermissionService.SaveUserPageOverridesAsync(model.UserId, model.Overrides ?? new List<UserPagePermissionDto>());
                
                return Ok(new { success = true, message = "Permissions saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving user permissions for user {model?.UserId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var usersQuery = @"
                    SELECT wm.WorkerID as UserId, 
                           wm.WorkerName as UserName,
                           wm.RoleID as RoleId,
                           rm.RoleName
                    FROM EHRMS.dbo.WorkerMaster wm
                    LEFT JOIN EHRMS.dbo.RoleMaster rm ON wm.RoleID = rm.RoleID
                    WHERE wm.WorkerID IS NOT NULL
                    ORDER BY wm.WorkerName";
                
                var users = await _unitOfWork.EHRMSLogin.GetTableData<dynamic>(usersQuery);
                
                if (users == null || !users.Any())
                {
                    return Json(new List<object>());
                }
                
                var userList = users.Select(u => new
                {
                    userId = u.UserId != null ? Convert.ToInt32(u.UserId) : 0,
                    userName = u.UserName?.ToString() ?? "Unknown",
                    roleId = u.RoleId != null ? Convert.ToInt32(u.RoleId) : 0,
                    roleName = u.RoleName?.ToString() ?? "No Role"
                }).ToList();
                
                return Json(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class UserPermissionSaveModel
    {
        public int UserId { get; set; }
        public List<UserPagePermissionDto>? Overrides { get; set; }
    }
}

