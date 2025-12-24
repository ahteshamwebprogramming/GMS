using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using GMS.Infrastructure.Models.EHRMSLogin;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.EHRMSLogin;
using GMS.Infrastructure.Models.RoleMenuMapping;
using System.Linq;

namespace GMS.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    public AccountController(ILogger<AccountController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
    }
    public IActionResult Login()
    {
        HttpContext.Session.Clear();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(EHRMSLoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        EHRMSLoginDTO? result = new EHRMSLoginDTO();
        EHRMSLoginWithChild? outputDTO = new EHRMSLoginWithChild();
        var res = await _accountsAPIController.GetLoginDetail(dto);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            outputDTO = (EHRMSLoginWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (outputDTO != null)
            {
                // Pass UserId to get effective permissions (role + user overrides)
                int userId = (int)(outputDTO.WorkerId ?? 0);
                List<MenuListDTO> menuListDTOs = await _accountsAPIController.GetMenuDetails(outputDTO.RoleId ?? 0, userId);

                HttpContext.Session.SetString("MenuList", JsonConvert.SerializeObject(menuListDTOs));

                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(outputDTO));

                // Determine first accessible menu link for this user
                var firstAccessibleMenu = menuListDTOs?
                    .Where(m =>
                        (m.IsActive ?? true) &&
                        !string.IsNullOrWhiteSpace(m.MenuLink) &&
                        (m.SelfMenu == false) &&
                        (m.MenuParentId.HasValue && m.MenuParentId > 0))
                    .OrderBy(m => m.SNo ?? int.MaxValue)
                    .ThenBy(m => m.Id)
                    .FirstOrDefault();

                var redirectUri = firstAccessibleMenu?.MenuLink ?? "/Home/AccessDenied";

                var claims = new List<Claim>      {
                new Claim(ClaimTypes.Name,outputDTO.WorkerName),
                new Claim(ClaimTypes.Role,outputDTO.RoleName),
                new Claim("WorkerId", outputDTO?.WorkerId?.ToString()),
                new Claim("Id", outputDTO?.WorkerId?.ToString()),
                
                //new Claim(ClaimTypes.Role,outputDTO.Role==null ? "Agent - Insurance" : objResultData.Role),
            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = redirectUri,
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return Redirect(redirectUri);
            }
            else
            {
                ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                return View();
            }
        }
        else
        {
            ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
            return View();
        }
    }


    //[HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Clear session data
        HttpContext.Session.Clear();

        // Sign out the user
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirect to the login page
        return RedirectToAction("Login", "Account");


    }
    [HttpPost]
    public IActionResult KeepAlive()
    {
        return Ok();
    }
}
