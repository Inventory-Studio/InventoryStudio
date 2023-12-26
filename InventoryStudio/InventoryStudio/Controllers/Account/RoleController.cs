using System.Linq;
using System.Threading.Tasks;
using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ISLibrary;
using System.Data;
using static CLRFramework.Database.Filter.StringSearch;

namespace InventoryStudio.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<User> _userManager;

        public RoleController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            
            //var userId = _userManager.GetUserId(User);

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

        public IActionResult Create()
        {
            return View("~/Views/Account/Role/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {          
            //var userId = _userManager.GetUserId(User);

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
                // 处理异常
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Account/Role/Create.cshtml");
            }
        }

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

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id.ToString());

        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(role);
        //}

        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id.ToString());

        //    if (role != null)
        //    {
        //        var result = await _roleManager.DeleteAsync(role);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }

        //    return View();
        //}

        //public async Task<IActionResult> AssignPermissions(string id)
        //{
        //    var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
        //    var role = new Role(id);

        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    if (role.CompanyId != organizationClaim.Value)
        //    {
        //        TempData["ErrorMessage"] = "You don't have permission to change other company's role.";
        //        return RedirectToAction("Index");
        //    }

        //    var permissions = Permission.GetPermissions();

        //    var model = new AssignPermissionsViewModel
        //    {
        //        RoleId = id,
        //        RoleName = role.Name,
        //        Permissions = permissions,
        //        AssignedPermissions = role.AssignedPermissionIds
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AssignPermissions(AssignPermissionsViewModel model)
        //{
        //    var role = await _roleManager.Roles
        //           .Include(r => r.RolePermissions)
        //               .ThenInclude(rp => rp.Permission)
        //           .FirstOrDefaultAsync(r => r.Id == model.RoleId);

        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update role permissions
        //    role.RolePermissions = model.SelectedPermissionIds.Select(permissionId =>
        //        new RolePermission { RoleId = model.RoleId, PermissionId = permissionId }).ToList();

        //    await _roleManager.UpdateAsync(role);

        //    return RedirectToAction("Index");
        //}


    }

}