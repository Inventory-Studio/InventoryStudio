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

        public RoleController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [Authorize(Policy = "Account-Role-List")]
        public IActionResult Index()
        {

            // 通过用户的 Claims 获取组织信息
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                RoleFilter filter = new RoleFilter();
                filter.CompanyId = new SearchFilter();
                filter.CompanyId.SearchString = organizationClaim.Value;
                var roles = Role.GetRoles(filter);
                return View("~/Views/Account/Role/Index.cshtml",roles);
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
            var role = new Role
            {
                Name = roleName,
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
            var role = new Role(id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }   

            

            return View("~/Views/Account/Role/Edit.cshtml",role);
        }

        [Authorize(Policy = "Account-Role-Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(Role FormRole)
        {
            var role = new Role(FormRole.Id);

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

            var role = new Role(id);

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

        public async Task<IActionResult> AssignPermissions(string id)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new Role(id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.CompanyId != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
                return RedirectToAction("Index");
            }

            var permissions = Permission.GetPermissions();

            var model = new AssignPermissionsViewModel
            {
                RoleId = id,
                RoleName = role.Name,
                Permissions = permissions,
                AssignPermissions = role.AssignPermissionIds
            };

            return View("~/Views/Account/Role/AssignPermissions.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissions(AssignPermissionsViewModel model)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var role = new Role(model.RoleId);

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
                RolePermission rolePermission = new RolePermission();
                rolePermission.PermissionId = permissionId;
                rolePermission.RoleId = model.RoleId;
                rolePermission.Create();
            }

           
            foreach (var permissionId in permissionsToRemove)
            {
                RolePermission rolePermission = new RolePermission(permissionId, model.RoleId);
                rolePermission.Delete();
            }

            return RedirectToAction("Index");
        }


    }

}