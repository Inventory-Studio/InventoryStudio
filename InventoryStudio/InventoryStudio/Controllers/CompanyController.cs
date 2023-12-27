using InventoryStudio.Models;
using ISLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers
{
    public class CompanyController : Controller
    {
        private readonly UserManager<User> _userManager;

        public CompanyController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var companies = Company.GetCompanies();
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user.UserType.Equals("3PL"))
            {
                var filter = new CompanyFilter();
                filter.CreatedBy = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CreatedBy.SearchString = userId;
                filter.ParentCompanyID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ParentCompanyID.Operator = CLRFramework.Database.Filter.StringSearch.SearchOperator.empty;
                var companies = Company.GetCompanies(filter);
                if (companies.Count > 0)
                    company.ParentCompanyID = companies[0].CompanyID;
            }
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");
            if (ModelState.IsValid)
            {
                company.CompanyGUID = Guid.NewGuid().ToString();
                company.CreatedOn = DateTime.Now;
                company.CreatedBy = Convert.ToInt32(userId);
                company.Create();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var company = new Company(id);
            if (company == null)
                return NotFound();
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Company company)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");

            if (string.IsNullOrEmpty(id))
                return NotFound();
            company.UpdatedBy = Convert.ToInt32(userId);
            company.Update();
            var current = new Company(id);
            if (current == null)
                return NotFound();
            return RedirectToAction(nameof(Index));
        }


    }
}
