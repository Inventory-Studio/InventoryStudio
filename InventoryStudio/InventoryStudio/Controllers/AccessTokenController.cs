using Azure.Core;
using Humanizer;
using InventoryStudio.Models;
using InventoryStudio.Models.AccessToken;
using ISLibrary;
using ISLibrary.AspNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InventoryStudio.Controllers
{
    public class AccessTokenController : Controller
    {
        public IActionResult Index()
        {
            //ToDo Need to optimize query
            var accessTokens = AspNetAccessTokens.GetAspNetUserAccessTokens();
            var list = new List<AccessTokenViewModel>();
            foreach (var accessToken in accessTokens)
            {
                list.Add(AccessTokenConvertViewModel(accessToken));
            }
            return View(list);
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var accessToken = new AspNetAccessTokens(id);
            if (accessToken == null)
                return NotFound();
            //ToDo Need to optimize query
            var detailViewModel = AccessTokenConvertViewModel(accessToken);
            return View(detailViewModel);
        }

        private AccessTokenViewModel AccessTokenConvertViewModel(AspNetAccessTokens accessToken)
        {
            var viewModel = new AccessTokenViewModel();
            viewModel.Id = accessToken.AccessTokenID;
            viewModel.ApplicationName = accessToken.ApplicationName;
            viewModel.TokenName = accessToken.TokenName;
            viewModel.InActive = accessToken.InActive;
            viewModel.Token = accessToken.Token;
            viewModel.Secret = accessToken.Secret;
            viewModel.CreatedOn = accessToken.CreatedOn;
            viewModel.UpdatedOn = accessToken.UpdatedOn;
            if (!string.IsNullOrEmpty(accessToken.CreatedBy))
            {
                var createdUser = new AspNetUsers(accessToken.CreatedBy);
                if (createdUser != null)
                    viewModel.CreatedBy = createdUser.UserName;
            }
            if (!string.IsNullOrEmpty(accessToken.UpdatedBy))
            {
                var updatedUser = new AspNetUsers(accessToken.UpdatedBy);
                if (updatedUser != null)
                    viewModel.UpdatedBy = updatedUser.UserName;
            }
            if (!string.IsNullOrEmpty(accessToken.RoleId))
            {
                var role = new AspNetRoles(accessToken.RoleId);
                if (role != null)
                    viewModel.Role = role.Name;
            }
            return viewModel;
        }

        public IActionResult Create()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var roles = AspNetRoles.GetAspNetRoless(company.Value);
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationName,TokenName,InActive,RoleId")] CreateAccessTokenViewModel input)
        {
            if (ModelState.IsValid)
            {
                var accessToken = new AspNetAccessTokens();
                accessToken.ApplicationName = input.ApplicationName;
                accessToken.TokenName = input.TokenName;
                accessToken.Token = Guid.NewGuid().ToString();//ToDo
                accessToken.Secret = Guid.NewGuid().ToString();//ToDo
                accessToken.InActive = input.InActive;
                accessToken.RoleId = input.RoleId;
                accessToken.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                accessToken.Create();
                return RedirectToAction(nameof(Index));
            }
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var roles = AspNetRoles.GetAspNetRoless(company.Value);
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name", input.RoleId);
            return View(input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var accessToken = new AspNetAccessTokens(id);
            if (accessToken == null)
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var roles = AspNetRoles.GetAspNetRoless(company.Value);
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name", accessToken.RoleId);
            var editViewModel = new EditAccessTokenViewModel();
            editViewModel.Id = accessToken.AccessTokenID;
            editViewModel.ApplicationName = accessToken.ApplicationName;
            editViewModel.TokenName = accessToken.TokenName;
            editViewModel.InActive = accessToken.InActive;
            editViewModel.RoleId = accessToken.RoleId;
            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ApplicationName,TokenName,InActive,RoleId")] EditAccessTokenViewModel input)
        {
            if (id != input.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                var accessToken = new AspNetAccessTokens(id);
                if (accessToken == null) return NotFound();
                accessToken.ApplicationName = input.ApplicationName;
                accessToken.TokenName = input.TokenName;
                accessToken.InActive = input.InActive;
                accessToken.RoleId = input.RoleId;
                accessToken.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                accessToken.Update();
                return RedirectToAction(nameof(Index));
            }
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var roles = AspNetRoles.GetAspNetRoless(company.Value);
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name", input.RoleId);
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var accessToken = new AspNetAccessTokens(id);
            if (accessToken == null) return NotFound();
            var viewModel = AccessTokenConvertViewModel(accessToken);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null) return NotFound();
            var accessToken = new AspNetAccessTokens(id);
            if (accessToken == null) return NotFound();
            accessToken.Delete();
            return RedirectToAction(nameof(Index));
        }
    }
}
