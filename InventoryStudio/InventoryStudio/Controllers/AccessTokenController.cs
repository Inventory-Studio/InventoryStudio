using Azure.Core;
using Humanizer;
using InventoryStudio.Models;
using InventoryStudio.Models.AccessToken;
using InventoryStudio.Services.Authorization;
using ISLibrary;
using ISLibrary.AspNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers
{
    public class AccessTokenController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public AccessTokenController(IAuthorizationService authorizationService) =>
            _authorizationService = authorizationService;

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
        public async Task<IActionResult> Create(
            [Bind("ApplicationName,TokenName,InActive,RoleId")]
            CreateAccessTokenViewModel input)
        {
            if (ModelState.IsValid)
            {
                var tokenResult = await _authorizationService.GenerateTokenByRole(input.RoleId);
                if (tokenResult.IsSuccess)
                {
                    var accessToken = new AspNetAccessTokens();
                    accessToken.AccessTokenID = tokenResult.TokenId;
                    accessToken.ApplicationName = input.ApplicationName;
                    accessToken.TokenName = input.TokenName;
                    accessToken.Token = tokenResult.Token;
                    accessToken.Secret = Guid.NewGuid().ToString(); //ToDo
                    accessToken.InActive = input.InActive;
                    accessToken.RoleId = input.RoleId;
                    accessToken.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    accessToken.Create();
                    return RedirectToAction(nameof(Index));
                }
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
        public async Task<IActionResult> Edit(string id,
            [Bind("Id,ApplicationName,TokenName,InActive,RoleId")]
            EditAccessTokenViewModel input)
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


        public IActionResult Insert([FromBody] CRUDModel<AspNetAccessTokens> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<AspNetAccessTokens> value)
        {
            return Json(value.Value ?? new AspNetAccessTokens());
        }

        public IActionResult Remove([FromBody] CRUDModel<AspNetAccessTokens> value)
        {
            if (value.Key != null)
            {
                DeleteConfirmed(value.Key.ToString() ?? "");
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AccessTokenViewModel> dataSource = new List<AccessTokenViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    AspNetAccessTokensFilter aspNetAccessTokensFilter = new();
                    dataSource = AspNetAccessTokens.GetAspNetUserAccessTokens(
                        aspNetAccessTokensFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    ).Select(AccessTokenConvertViewModel);
                }
            }

            if (dm.Search != null && dm.Search.Count > 0)
            {
                dataSource = operation.PerformSearching(dataSource, dm.Search); //Search
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            }

            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                if (dm.Where != null && dm.Where.Any()) //Filtering
                {
                    foreach (WhereFilter whereFilter in dm.Where)
                    {
                        if (whereFilter.IsComplex)
                        {
                            foreach (WhereFilter whereFilterPredicate in whereFilter.predicates)
                            {
                                dataSource = operation.PerformFiltering(
                                    dataSource,
                                    dm.Where,
                                    whereFilterPredicate.Operator
                                );
                            }
                        }
                        else
                        {
                            dataSource = operation.PerformFiltering(
                                dataSource,
                                dm.Where,
                                dm.Where.First().Operator
                            );
                        }
                    }
                }
            }

            if (dm.Skip != 0)
            {
                dataSource = operation.PerformSkip(dataSource, dm.Skip); //Paging
            }

            if (dm.Take != 0)
            {
                dataSource = operation.PerformTake(dataSource, dm.Take);
            }

            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = null };
            return dm.RequiresCounts
                ? Json(new { result = dataSource, count = totalRecord }, jsonSerializerOptions)
                : Json(dataSource, jsonSerializerOptions);
        }
    }
}