using InventoryStudio.Models.OrderManagement.PackageDimension;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class PackageDimensionController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public PackageDimensionController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim? company = user.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                    CompanyID = company.Value;
            }
        }

        public IActionResult Index()
        {
            var packageDimensions = PackageDimension.GetPackageDimensions(CompanyID);
            var list = new List<PackageDimensionViewModel>();
            foreach (var packageDimension in packageDimensions)
            {
                list.Add(EntitieConvertViewModel(packageDimension));
            }

            return View("~/Views/OrderManagement/PackageDimension/Index.cshtml", list);
        }

        private PackageDimensionViewModel EntitieConvertViewModel(PackageDimension packageDimension)
        {
            var viewModel = new PackageDimensionViewModel();
            viewModel.PackageDimensionID = packageDimension.PackageDimensionID;
            if (string.IsNullOrEmpty(packageDimension.CompanyID))
            {
                var company = new Company(packageDimension.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }

            viewModel.Name = packageDimension.Name;
            viewModel.Length = packageDimension.Length;
            viewModel.Width = packageDimension.Width;
            viewModel.Height = packageDimension.Height;
            viewModel.Weight = packageDimension.Weight;
            viewModel.WeightUnit = packageDimension.WeightUnit;
            viewModel.Cost = packageDimension.Cost;
            viewModel.ShippingPackage = packageDimension.ShippingPackage;
            viewModel.Template = packageDimension.Template;
            return viewModel;
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var packageDimension = new PackageDimension(CompanyID, id);
            if (packageDimension == null)
                return NotFound();
            var detailViewModel = EntitieConvertViewModel(packageDimension);
            return View("~/Views/OrderManagement/PackageDimension/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/PackageDimension/Create.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("CompanyID,Name,Length,Width,Height,Weight,WeightUnit,Cost,ShippingPackage,Template")]
            CreatePackageDimensionViewModel input)
        {
            if (ModelState.IsValid)
            {
                var packageDimension = new PackageDimension();
                packageDimension.CompanyID = input.CompanyID;
                packageDimension.Name = input.Name;
                packageDimension.Length = input.Length;
                packageDimension.Width = input.Width;
                packageDimension.Height = input.Height;
                packageDimension.Weight = input.Weight;
                packageDimension.WeightUnit = input.WeightUnit;
                packageDimension.Cost = input.Cost;
                packageDimension.ShippingPackage = input.ShippingPackage;
                packageDimension.Template = input.Template;
                packageDimension.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                packageDimension.Create();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/PackageDimension/Create.cshtml", input);
        }


        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var packageDimension = new PackageDimension(CompanyID, id);
            if (packageDimension == null)
                return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            var viewModel = new EditPackageDimensionViewModel();
            viewModel.PackageDimensionID = packageDimension.PackageDimensionID;
            viewModel.CompanyID = packageDimension.CompanyID;
            viewModel.Name = packageDimension.Name;
            viewModel.Length = packageDimension.Length;
            viewModel.Width = packageDimension.Width;
            viewModel.Height = packageDimension.Height;
            viewModel.Weight = packageDimension.Weight;
            viewModel.WeightUnit = packageDimension.WeightUnit;
            viewModel.Cost = packageDimension.Cost;
            viewModel.ShippingPackage = packageDimension.ShippingPackage;
            viewModel.Template = packageDimension.Template;
            return View("~/Views/OrderManagement/PackageDimension/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] string id,
            [Bind(
                "PackageDimensionID,CompanyID,Name,Length,Width,Height,Weight,WeightUnit,Cost,ShippingPackage,Template")]
            EditPackageDimensionViewModel input)
        {
            if (id != input.PackageDimensionID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var packageDimension = new PackageDimension();
                packageDimension.PackageDimensionID = input.PackageDimensionID;
                packageDimension.CompanyID = input.CompanyID;
                packageDimension.Name = input.Name;
                packageDimension.Length = input.Length;
                packageDimension.Width = input.Width;
                packageDimension.Height = input.Height;
                packageDimension.Weight = input.Weight;
                packageDimension.WeightUnit = input.WeightUnit;
                packageDimension.Cost = input.Cost;
                packageDimension.ShippingPackage = input.ShippingPackage;
                packageDimension.Template = input.Template;
                packageDimension.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                packageDimension.Update();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/PackageDimension/Edit.cshtml", input);
        }


        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();

            var packageDimension = new PackageDimension(CompanyID, id);
            var viewModel = EntitieConvertViewModel(packageDimension);
            return View("~/Views/OrderManagement/PackageDimension/Delete.cshtml", viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var packageDimension = new PackageDimension(CompanyID, id);
            if (packageDimension != null)
                packageDimension.Delete();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Insert([FromBody] CRUDModel value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<PackageDimensionViewModel> value)
        {
            return Json(value.Value ?? new PackageDimensionViewModel());
        }


        public IActionResult Remove([FromBody] CRUDModel<PackageDimensionViewModel> value)
        {
            if (value.Key != null)
            {
                var id = value.Key.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    DeleteConfirmed(id);
                }
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<PackageDimensionViewModel> dataSource = new List<PackageDimensionViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    PackageDimensionFilter packageDimensionFilter = new();
                    dataSource = PackageDimension.GetPackageDimensions(
                        CompanyID,
                        packageDimensionFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    ).Select(EntitieConvertViewModel);
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