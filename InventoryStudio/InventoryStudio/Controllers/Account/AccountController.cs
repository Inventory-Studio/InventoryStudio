using InventoryStudio.Data;
using InventoryStudio.Models;
using ISLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Data;
using System.Text.Json;


namespace InventoryStudio.Controllers
{
    public class AccountController : BaseController
    {
        private UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(
             UserManager<User> userManager,
        SignInManager<User> signInManager,
        ApplicationDbContext context 
            ) : base ()
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> SelectCompany()
        {
            var user = await _userManager.GetUserAsync(User);

            List<Company> companies = user.AspNetUser.Companies;

            if(companies.Count == 1)
            {
                return await SwitchCompany( int.Parse(companies[0].CompanyID));
            }

            return View("~/Views/Account/Account/SelectCompany.cshtml", companies);
        }

        [HttpPost]
        public async Task<IActionResult> SwitchCompany(int companyId)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.GetUserAsync(User);

            List<Company> companies = user.AspNetUser.Companies;

            if (companies.Any(c => c.CompanyID == companyId.ToString()))
            {
                await SwitchToCompany(int.Parse(userId), companyId);

                return  RedirectToAction("SelectRole","Account");
            }
            else
            {
                ViewBag.ErrorMessage = "You don't have permission for this company!";
                return View("~/Views/Account/Account/SelectCompany.cshtml", companies);
            }           
        }

        private async Task SwitchToCompany(int userId, int comapnyId)
        {
                      
            //add current companyId
            var existingOrganizationClaim = _context.UserClaims
                .FirstOrDefault(c => c.UserId == userId && c.ClaimType == "CompanyId");

            if (existingOrganizationClaim != null)
            {
                existingOrganizationClaim.ClaimValue = comapnyId.ToString();
                _context.UserClaims.Update(existingOrganizationClaim);
            }
            else
            {
                var newOrganizationClaim = new IdentityUserClaim<int>
                {
                    UserId = userId,
                    ClaimType = "CompanyId",
                    ClaimValue = comapnyId.ToString()
                };
                _context.UserClaims.Add(newOrganizationClaim);
            }

            //add RootUser Claim 
            var existingRootUserClaim = _context.UserClaims
               .FirstOrDefault(c => c.UserId == userId && c.ClaimType == "RootUser");

            var user = await _userManager.GetUserAsync(User);
            List<Company> companies = user.AspNetUser.Companies;

            List<string> companyIds = companies.Where(c => c.CreatedBy == userId)
                          .Select(c => c.CompanyID)
                          .ToList();

            if (existingRootUserClaim != null)
            {                
                List<string> SavedCompanyIds = JsonSerializer.Deserialize<List<string>>(existingRootUserClaim.ClaimValue);
                var differentElements = companyIds.Except(SavedCompanyIds).ToList();

                if (differentElements.Any() || companyIds.Count != SavedCompanyIds.Count)
                {
                    existingRootUserClaim.ClaimValue = JsonSerializer.Serialize(companyIds);
                    _context.UserClaims.Update(existingRootUserClaim);
                }                            
            }
            else
            {
                if (companyIds.Count > 0)
                {
                    var newRootUserClaim = new IdentityUserClaim<int>
                    {
                        UserId = userId,
                        ClaimType = "RootUser",
                        ClaimValue = JsonSerializer.Serialize(companyIds)
                };
                    _context.UserClaims.Add(newRootUserClaim);
                }                
            }

            _context.SaveChanges();            

            await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(userId.ToString()));

        }


        public async Task<IActionResult> SelectRole()
        {
            var user = await _userManager.GetUserAsync(User);

            List<AspNetRoles> roles = user.AspNetUser.Roles;

            if (roles.Count == 0)
            {
                return RedirectToAction("Index","Home");
            }

            if (roles.Count == 1)
            {
                return await SwitchRole(roles[0].Id);
            }

            return View("~/Views/Account/Account/SelectRoles.cshtml", roles);
        }

        [HttpPost]
        public async Task<IActionResult> SwitchRole(string roleId)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.GetUserAsync(User);

            List<AspNetRoles> roles = user.AspNetUser.Roles;

            AspNetRoles role = roles.FirstOrDefault(c => c.Id == roleId);

            if (role != null)
            {
                await SwitchToRole(int.Parse(userId), role);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "You don't have permission for this Role!";
                return View("~/Views/Account/Account/SelectRoles.cshtml", roles);
            }
        }

        private async Task SwitchToRole(int userId, AspNetRoles role)
        {
            //add currentRole
            var existingRoleClaim = _context.UserClaims
                .FirstOrDefault(c => c.UserId == userId && c.ClaimType == "Role");

            if (existingRoleClaim != null)
            {
                existingRoleClaim.ClaimValue = role.Name;
                _context.UserClaims.Update(existingRoleClaim);
            }
            else
            {
                var roleClaim = new IdentityUserClaim<int>
                {
                    UserId = userId,
                    ClaimType = "Role",
                    ClaimValue = role.Name
                };
                _context.UserClaims.Add(roleClaim);
            }

            // get current permissions
            var existingPermissionClaims = _context.UserClaims
                .Where(c => c.UserId == userId && c.ClaimType == "Permission")
                .ToList();

            // Remove all of the Permission
            _context.UserClaims.RemoveRange(existingPermissionClaims);

            List<AspNetPermission> rolePermissions = role.AssignPermissions;

            // Save Permission into Claims 

            string permissionsJson = JsonSerializer.Serialize(rolePermissions.Select(p => p.Name).ToList());
           
            var permissionClaim = new IdentityUserClaim<int>
            {
                UserId = userId,
                ClaimType = "Permission",
                ClaimValue = permissionsJson
            };
            _context.UserClaims.Add(permissionClaim);
                       

            // 保存更改
            _context.SaveChanges();

            await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(userId.ToString()));
        }

        

    }
}
