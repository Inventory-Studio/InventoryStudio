﻿using InventoryStudio.Data;
using InventoryStudio.Models;
using InventoryStudio.Models.Company;
using ISLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public CompanyController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");
            var user = await _userManager.GetUserAsync(User);
            List<Company> companies = user.AspNetUser.Companies;
            return View(companies);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user.UserType.Equals("Normal"))
            {
                var filter = new CompanyFilter();
                filter.CreatedBy = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CreatedBy.SearchString = userId;
                var companies = Company.GetCompanies(filter);
                if (companies.Count > 0)
                {
                    ViewBag.ErrorMessage = "Normal users can only create one company";
                    return View("Error");
                }
            }

            CreateCompanyViewModel viewModel = new CreateCompanyViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyViewModel input)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            Company parentCompany = null;
            if (user.UserType.Equals("3PL"))
            {
                var filter = new CompanyFilter();
                filter.CreatedBy = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CreatedBy.SearchString = userId;
                filter.ParentCompanyID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ParentCompanyID.Operator = CLRFramework.Database.Filter.StringSearch.SearchOperator.empty;
                var companies = Company.GetCompanies(filter);
                if (companies.Count > 0)
                    parentCompany = companies[0];
            }

            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");
            if (ModelState.IsValid)
            {
                Company company = new Company();
                company.CompanyName = input.CompanyName;
                company.AutomateFulfillment = input.AutomateFulfillment;
                company.ShippoAPIKey = input.ShippoAPIKey;
                company.IncludePackingSlipOnLabel = input.IncludePackingSlipOnLabel;
                company.DefaultFulfillmentMethod = input.DefaultFulfillmentMethod;
                company.DefaultFulfillmentStrategy = input.DefaultFulfillmentStrategy;
                company.DefaultAllocationStrategy = input.DefaultAllocationStrategy;
                company.CompanyGUID = Guid.NewGuid().ToString();
                if (parentCompany != null)
                    company.ParentCompanyID = parentCompany.CompanyID;
                company.CreatedOn = DateTime.Now;
                company.CreatedBy = Convert.ToInt32(userId);
                company.Create();
                AspNetUserCompany aspNetUserCompany = new AspNetUserCompany();
                aspNetUserCompany.UserId = userId;
                aspNetUserCompany.CompanyID = company.CompanyID;
                aspNetUserCompany.Create();

                var existingOrganizationClaim = _context.UserClaims
                    .FirstOrDefault(c => c.UserId == Convert.ToInt32(userId) && c.ClaimType == "CompanyId");

                if (existingOrganizationClaim != null)
                {
                    existingOrganizationClaim.ClaimValue = company.CompanyID;
                    _context.UserClaims.Update(existingOrganizationClaim);
                }
                else
                {
                    var newOrganizationClaim = new IdentityUserClaim<int>
                    {
                        UserId = Convert.ToInt32(userId),
                        ClaimType = "CompanyId",
                        ClaimValue = company.CompanyID
                    };
                    _context.UserClaims.Add(newOrganizationClaim);
                }

                return RedirectToAction("SelectCompany", "Account");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var company = new Company(id);
            if (company == null)
                return NotFound();
            var editViewModel = new EditCompanyViewModel();
            editViewModel.CompanyID = company.CompanyID;
            editViewModel.CompanyName = company.CompanyName;
            editViewModel.AutomateFulfillment = company.AutomateFulfillment;
            editViewModel.ShippoAPIKey = company.ShippoAPIKey;
            editViewModel.IncludePackingSlipOnLabel = company.IncludePackingSlipOnLabel;
            editViewModel.DefaultFulfillmentMethod = company.DefaultFulfillmentMethod;
            editViewModel.DefaultFulfillmentStrategy = company.DefaultFulfillmentStrategy;
            editViewModel.DefaultAllocationStrategy = company.DefaultAllocationStrategy;
            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditCompanyViewModel input)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var company = new Company(id);
            if (company == null)
                return NotFound();

            company.CompanyName = input.CompanyName;
            company.AutomateFulfillment = input.AutomateFulfillment;
            company.ShippoAPIKey = input.ShippoAPIKey;
            company.DefaultFulfillmentMethod = input.DefaultFulfillmentMethod;
            company.DefaultFulfillmentStrategy = input.DefaultFulfillmentStrategy;
            company.DefaultAllocationStrategy = input.DefaultAllocationStrategy;
            company.IncludePackingSlipOnLabel = input.IncludePackingSlipOnLabel;
            company.UpdatedBy = Convert.ToInt32(userId);
            company.Update();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var company = new Company(id);
            if (company == null)
                return NotFound();
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var company = new Company(id);
            if (company == null)
                return NotFound();
            company.Delete();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var company = new Company(id);
            if (company == null)
                return NotFound();
            return View(company);
        }

        public IActionResult Insert([FromBody] CRUDModel<AspNetUsers> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<AspNetUsers> value)
        {
            return Json(value.Value ?? new AspNetUsers());
        }

        public IActionResult Remove([FromBody] CRUDModel<AspNetUsers> value)
        {
            if (value.Key != null)
            {
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<Company> dataSource = new List<Company>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    CompanyFilter companyFilter = new();
                    dataSource = Company.GetCompanies(
                        companyFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    );
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