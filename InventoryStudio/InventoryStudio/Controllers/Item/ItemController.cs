using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ISLibrary;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace InventoryStudio.Controllers
{
    public class ItemController : Controller

    { 
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ItemController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "Item-Item-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                var items = Item.GetItems(organizationClaim.Value);

                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Create")).Succeeded,
                    ["CanEdit"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Edit")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Delete")).Succeeded,
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Item/Item/Index.cshtml", items);
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }


        [Authorize(Policy = "Item-Item-Create")]
        public IActionResult Create()
        {
            return View("~/Views/Item/Item/Create.cshtml");
        }

        [Authorize(Policy = "Item-Item-Create")]
        [HttpPost]
        public IActionResult Create(string roleName)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return View("~/Views/Item/Item/Create.cshtml");
            }

            var role = new AspNetRoles
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                CompanyId = organizationClaim.Value
            };

            try
            {
                role.Create();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Item/Item/Create.cshtml");
            }
        }

    }
}
