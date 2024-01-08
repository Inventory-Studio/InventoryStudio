using System.Linq;
using System.Threading.Tasks;
using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ISLibrary;
using System.Data;
using static CLRFramework.Database.Filter.StringSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace InventoryStudio.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public RoleController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }


        [Authorize(Policy = "Account-Role-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                var roles = AspNetRoles.GetAspNetRoless(organizationClaim.Value);

                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Account-Role-Create")).Succeeded,
                    ["CanEdit"] = (await _authorizationService.AuthorizeAsync(User, "Account-Role-Edit")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Account-Role-Delete")).Succeeded,
                    ["CanAssignPermission"] = (await _authorizationService.AuthorizeAsync(User, "Account-Role-AssignPermissions")).Succeeded,
                    ["CanAssignUser"] = (await _authorizationService.AuthorizeAsync(User, "Account-Role-AssignUsers")).Succeeded
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Account/Role/Index.cshtml", roles);
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }

        [Authorize(Policy = "Account-Role-Create")]
        public IActionResult Create()
        {
            return View("~/Views/Account/Role/Create.cshtml");
        }

        [Authorize(Policy = "Account-Role-Create")]
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return View("~/Views/Account/Role/Create.cshtml");
            }
            var role = new AspNetRoles
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                CompanyId = organizationClaim.Value
            };

            try
            {
                var result = role.Create();


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Account/Role/Create.cshtml");
            }
        }

        [Authorize(Policy = "Account-Role-Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new AspNetRoles(id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            var allUsers = AspNetUsers.GetAspNetUserss(role.CompanyId);
            var permissions = AspNetPermission.GetAspNetPermissions();

            var model = new RoleManagementViewModel
            {
                AspNetRoles = role,
                AssignUsersViewModel = new AssignUsersViewModel
                {
                    RoleId = id,
                    RoleName = role.Name,
                    Users = allUsers,
                    AssignedUserIds = role.AssignUserIds
                },
                AssignPermissionsViewModel = new AssignPermissionsViewModel
                {
                    RoleId = id,
                    RoleName = role.Name,
                    Permissions = permissions,
                    AssignPermissions = role.AssignPermissionIds
                }
            };

            return View("~/Views/Account/Role/Edit.cshtml", model);
        }

        [Authorize(Policy = "Account-Role-Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(RoleManagementViewModel roleManagementViewModel)
        {
            var FormRole = roleManagementViewModel.AspNetRoles;
            var role = new AspNetRoles(FormRole.Id);

            if (role == null)
            {
                return NotFound();
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return View("~/Views/Account/Role/Edit.cshtml", role);
            }

            role.Name = FormRole.Name;

            try
            {
                var result = role.Update();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // 处理异常
                ModelState.AddModelError("update_error", ex.Message);
                return View("~/Views/Account/Role/Edit.cshtml", role);
            }
        }

        [Authorize(Policy = "Account-Role-Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = new AspNetRoles(id);

            if (role == null)
            {
                return NotFound();
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            role.Delete();

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "Account-Role-AssignPermissions")]
        public async Task<IActionResult> AssignPermissions(string id)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new AspNetRoles(id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            var permissions = AspNetPermission.GetAspNetPermissions();

            var model = new AssignPermissionsViewModel
            {
                RoleId = id,
                RoleName = role.Name,
                Permissions = permissions,
                AssignPermissions = role.AssignPermissionIds
            };

            return View("~/Views/Account/Role/AssignPermissions.cshtml", model);
        }

        [Authorize(Policy = "Account-Role-AssignPermissions")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissions(AssignPermissionsViewModel model)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new AspNetRoles(model.RoleId);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            // Update role permissions

            var currentPermissions = role.AssignPermissionIds ?? new List<string>();

            var selectedPermissions = model.SelectedPermissionIds ?? new List<string>();

            var permissionsToAdd = selectedPermissions.Except(currentPermissions).ToList();

            var permissionsToRemove = currentPermissions.Except(selectedPermissions).ToList();

            foreach (var permissionId in permissionsToAdd)
            {
                AspNetRolePermission rolePermission = new AspNetRolePermission();
                rolePermission.PermissionId = permissionId;
                rolePermission.RoleId = model.RoleId;
                rolePermission.Create();
            }


            foreach (var permissionId in permissionsToRemove)
            {
                AspNetRolePermission rolePermission = new AspNetRolePermission(model.RoleId, permissionId);
                rolePermission.Delete();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "Account-Role-AssignUsers")]
        public async Task<IActionResult> AssignUsers(string id)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new AspNetRoles(id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            var allUsers = AspNetUsers.GetAspNetUserss(role.CompanyId);

            var model = new AssignUsersViewModel
            {
                RoleId = id,
                RoleName = role.Name,
                Users = allUsers,
                AssignedUserIds = role.AssignUserIds
            };

            return View("~/Views/Account/Role/AssignUsers.cshtml", model);
        }

        [Authorize(Policy = "Account-Role-AssignUsers")]
        [HttpPost]
        public async Task<IActionResult> AssignUsers(AssignUsersViewModel model)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new AspNetRoles(model.RoleId);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            var currentUsers = role.AssignUserIds ?? new List<string>();
            var selectedUsers = model.SelectedUserIds ?? new List<string>();

            var usersToAdd = selectedUsers.Except(currentUsers).ToList();
            var usersToRemove = currentUsers.Except(selectedUsers).ToList();

            foreach (var userId in usersToAdd)
            {
                AspNetUserRoles userRole = new AspNetUserRoles();
                userRole.UserId = userId;
                userRole.RoleId = model.RoleId;
                userRole.Create();
            }

            foreach (var userId in usersToRemove)
            {
                AspNetUserRoles userRole = new AspNetUserRoles(userId, model.RoleId);
                userRole.Delete();
            }

            return RedirectToAction("Index");
        }



    }

}