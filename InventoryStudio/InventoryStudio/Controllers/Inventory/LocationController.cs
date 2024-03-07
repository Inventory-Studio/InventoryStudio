using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;
using InventoryStudio.Models;
using Microsoft.AspNetCore.Authorization;

namespace InventoryStudio.Controllers
{
    public class LocationController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public LocationController(IHttpContextAccessor httpContextAccessor)
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

        [Authorize(Policy = "Inventory-Location-List")]
        public IActionResult Index()
        {
            var locations = Location.GetLocations(CompanyID);
            var list = new List<LocationViewModel>();
            foreach (var locaion in locations)
            {
                list.Add(EntityConvertViewModel(locaion));
            }
            return View("~/Views/Inventory/Location/Index.cshtml", list);
        }

        private LocationViewModel EntityConvertViewModel(Location location)
        {
            var viewModel = new LocationViewModel();
            viewModel.LocationID = location.LocationID;
            if (!string.IsNullOrEmpty(location.CompanyID))
            {
                var company = new Company(location.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }
            viewModel.ParentLocationID = location.ParentLocationID;
            viewModel.LocationNumber = location.LocationNumber;
            viewModel.LocationName = location.LocationName;
            viewModel.UseBins = location.UseBins;
            viewModel.UseLotNumber = location.UseLotNumber;
            viewModel.UseCartonNumber = location.UseCartonNumber;
            viewModel.UseVendorCartonNumber = location.UseVendorCartonNumber;
            viewModel.UseSerialNumber = location.UseSerialNumber;
            viewModel.AllowMultiplePackagePerFulfillment = location.AllowMultiplePackagePerFulfillment;
            viewModel.AllowAutoPick = location.AllowAutoPick;
            viewModel.AllowAutoPickApproval = location.AllowAutoPickApproval;
            viewModel.AllowNegativeInventory = location.AllowNegativeInventory;
            viewModel.DefaultAddressValidation = location.DefaultAddressValidation;
            viewModel.DefaultSignatureRequirement = location.DefaultSignatureRequirement;
            viewModel.DefaultSignatureRequirementAmount = location.DefaultSignatureRequirementAmount;
            viewModel.DefaultCountryOfOrigin = location.DefaultCountryOfOrigin;
            viewModel.DefaultHSCode = location.DefaultHSCode;
            viewModel.DefaultLowestShippingRate = location.DefaultLowestShippingRate;
            viewModel.MaximumPickScanRequirement = location.MaximumPickScanRequirement;
            viewModel.MaximumPackScanRequirement = location.MaximumPackScanRequirement;
            viewModel.DisplayWeightMode = location.DisplayWeightMode;
            viewModel.FulfillmentCombineStatus = location.FulfillmentCombineStatus;
            if (!string.IsNullOrEmpty(location.DefaultPackageDimensionID))
            {
                var packageDimension = new PackageDimension(CompanyID, location.DefaultPackageDimensionID.ToString());
                if (packageDimension != null)
                    viewModel.DefaultPackageDimension = packageDimension.Name;
            }
            viewModel.EnableSimpleMode = location.EnableSimpleMode;
            viewModel.EnableSimpleModePick = location.EnableSimpleModePick;
            viewModel.EnableSimpleModePack = location.EnableSimpleModePack;
            viewModel.ValidateSource = location.ValidateSource;
            if (!string.IsNullOrEmpty(location.AddressID))
            {
                var address = new Address(location.AddressID);
                if (address != null)
                    viewModel.Address = address.FullName;
            }
            viewModel.VarianceBinID = location.VarianceBinID;
            if (!string.IsNullOrEmpty(location.UpdatedBy))
            {
                var user = new AspNetUsers(location.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }

            viewModel.UpdatedOn = location.UpdatedOn;
            if (!string.IsNullOrEmpty(location.CreatedBy))
            {
                var user = new AspNetUsers(location.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }
            viewModel.CreatedOn = location.CreatedOn;

            var auditDataList = AuditData.GetAuditDatas("Location", viewModel.LocationID);
            viewModel.AuditDataList = auditDataList;
            return viewModel;
        }

        [Authorize(Policy = "Inventory-Location-View")]
        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var location = new Location(CompanyID, id);
            if (location == null)
                return NotFound();
            var detailViewModel = EntityConvertViewModel(location);
            detailViewModel.BinList = location.Bins;
            return View("~/Views/Inventory/Location/Details.cshtml", detailViewModel);
        }

        [Authorize(Policy = "Inventory-Location-Create")]
        public IActionResult Create()
        {
            var addresses = Address.GetAddresses(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var packageDimensions = PackageDimension.GetPackageDimensions(CompanyID);
            ViewData["AddressID"] = new SelectList(addresses, "AddressID", "FullName");
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            ViewData["DefaultPackageDimensionID"] = new SelectList(packageDimensions, "PackageDimensionID", "Name");
            return View("~/Views/Inventory/Location/Create.cshtml");
        }

        [Authorize(Policy = "Inventory-Location-Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateLocationViewModel input)
        {
            if (ModelState.IsValid)
            {
                var location = new Location();
                location.CompanyID = CompanyID;
                location.ParentLocationID = input.ParentLocationID;
                location.LocationNumber = input.LocationNumber;
                location.LocationName = input.LocationName;
                location.UseBins = input.UseBins;
                location.UseLotNumber = input.UseLotNumber;
                location.UseCartonNumber = input.UseCartonNumber;
                location.UseVendorCartonNumber = input.UseVendorCartonNumber;
                location.UseSerialNumber = input.UseSerialNumber;
                location.AllowMultiplePackagePerFulfillment = input.AllowMultiplePackagePerFulfillment;
                location.AllowAutoPick = input.AllowAutoPick;
                location.AllowAutoPickApproval = input.AllowAutoPickApproval;
                location.AllowNegativeInventory = input.AllowNegativeInventory;
                location.DefaultAddressValidation = input.DefaultAddressValidation;
                location.DefaultSignatureRequirement = input.DefaultSignatureRequirement;
                location.DefaultSignatureRequirementAmount = input.DefaultSignatureRequirementAmount;
                location.DefaultCountryOfOrigin = input.DefaultCountryOfOrigin;
                location.DefaultHSCode = input.DefaultHscode;
                location.DefaultLowestShippingRate = input.DefaultLowestShippingRate;
                location.MaximumPickScanRequirement = input.MaximumPickScanRequirement;
                location.MaximumPackScanRequirement = input.MaximumPackScanRequirement;
                location.DisplayWeightMode = input.DisplayWeightMode;
                location.FulfillmentCombineStatus = input.FulfillmentCombineStatus;
                location.DefaultPackageDimensionID = input.DefaultPackageDimensionID;
                location.EnableSimpleMode = input.EnableSimpleMode;
                location.EnableSimpleModePick = input.EnableSimpleModePick;
                location.EnableSimpleModePack = input.EnableSimpleModePack;
                location.ValidateSource = input.ValidateSource;
                location.AddressID = input.AddressID;
                location.VarianceBinID = input.VarianceBinID;
                location.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                location.Create();
                return RedirectToAction(nameof(Index));
            }
            var addresses = Address.GetAddresses(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var packageDimensions = PackageDimension.GetPackageDimensions(CompanyID);
            ViewData["AddressID"] = new SelectList(addresses, "AddressID", "FullName", input.AddressID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            ViewData["DefaultPackageDimensionID"] = new SelectList(packageDimensions, "PackageDimensionID", "Name", input.DefaultPackageDimensionID);
            return View("~/Views/Inventory/Location/Create.cshtml", input);
        }

        [Authorize(Policy = "Inventory-Location-Edit")]
        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var location = new Location(CompanyID, id);
            if (location == null)
                return NotFound();
            var addresses = Address.GetAddresses(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var packageDimensions = PackageDimension.GetPackageDimensions(CompanyID);
            ViewData["AddressID"] = new SelectList(addresses, "AddressID", "FullName");
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            ViewData["DefaultPackageDimensionID"] = new SelectList(packageDimensions, "PackageDimensionID", "Name");

            var viewModel = new EditLocationViewModel();
            viewModel.LocationID = location.LocationID;
            viewModel.ParentLocationID = location.ParentLocationID;
            viewModel.LocationNumber = location.LocationNumber;
            viewModel.LocationName = location.LocationName;
            viewModel.UseBins = location.UseBins;
            viewModel.UseLotNumber = location.UseLotNumber;
            viewModel.UseCartonNumber = location.UseCartonNumber;
            viewModel.UseVendorCartonNumber = location.UseVendorCartonNumber;
            viewModel.UseSerialNumber = location.UseSerialNumber;
            viewModel.AllowMultiplePackagePerFulfillment = location.AllowMultiplePackagePerFulfillment;
            viewModel.AllowAutoPick = location.AllowAutoPick;
            viewModel.AllowAutoPickApproval = location.AllowAutoPickApproval;
            viewModel.AllowNegativeInventory = location.AllowNegativeInventory;
            viewModel.DefaultAddressValidation = location.DefaultAddressValidation;
            viewModel.DefaultSignatureRequirement = location.DefaultSignatureRequirement;
            viewModel.DefaultSignatureRequirementAmount = location.DefaultSignatureRequirementAmount;
            viewModel.DefaultCountryOfOrigin = location.DefaultCountryOfOrigin;
            viewModel.DefaultHSCode = location.DefaultHSCode;
            viewModel.DefaultLowestShippingRate = location.DefaultLowestShippingRate;
            viewModel.MaximumPickScanRequirement = location.MaximumPickScanRequirement;
            viewModel.MaximumPackScanRequirement = location.MaximumPackScanRequirement;
            viewModel.DisplayWeightMode = location.DisplayWeightMode;
            viewModel.FulfillmentCombineStatus = location.FulfillmentCombineStatus;
            viewModel.DefaultPackageDimensionID = location.DefaultPackageDimensionID;
            viewModel.EnableSimpleMode = location.EnableSimpleMode;
            viewModel.EnableSimpleModePick = location.EnableSimpleModePick;
            viewModel.EnableSimpleModePack = location.EnableSimpleModePack;
            viewModel.ValidateSource = location.ValidateSource;
            viewModel.AddressID = location.AddressID;
            viewModel.VarianceBinID = location.VarianceBinID;
            return View("~/Views/Inventory/Location/Edit.cshtml", viewModel);
        }

        [Authorize(Policy = "Inventory-Location-Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditLocationViewModel input)
        {
           
            if (ModelState.IsValid)
            {
                var location = new Location(CompanyID, input.LocationID);
                if (location == null)
                    return NotFound();
                location.ParentLocationID = input.ParentLocationID;
                location.LocationNumber = input.LocationNumber;
                location.LocationName = input.LocationName;
                location.UseBins = input.UseBins;
                location.UseLotNumber = input.UseLotNumber;
                location.UseCartonNumber = input.UseCartonNumber;
                location.UseVendorCartonNumber = input.UseVendorCartonNumber;
                location.UseSerialNumber = input.UseSerialNumber;
                location.AllowMultiplePackagePerFulfillment = input.AllowMultiplePackagePerFulfillment;
                location.AllowAutoPick = input.AllowAutoPick;
                location.AllowAutoPickApproval = input.AllowAutoPickApproval;
                location.AllowNegativeInventory = input.AllowNegativeInventory;
                location.DefaultAddressValidation = input.DefaultAddressValidation;
                location.DefaultSignatureRequirement = input.DefaultSignatureRequirement;
                location.DefaultSignatureRequirementAmount = input.DefaultSignatureRequirementAmount;
                location.DefaultCountryOfOrigin = input.DefaultCountryOfOrigin;
                location.DefaultHSCode = input.DefaultHSCode;
                location.DefaultLowestShippingRate = input.DefaultLowestShippingRate;
                location.MaximumPickScanRequirement = input.MaximumPickScanRequirement;
                location.MaximumPackScanRequirement = input.MaximumPackScanRequirement;
                location.DisplayWeightMode = input.DisplayWeightMode;
                location.FulfillmentCombineStatus = input.FulfillmentCombineStatus;
                location.DefaultPackageDimensionID = input.DefaultPackageDimensionID;
                location.EnableSimpleMode = input.EnableSimpleMode;
                location.EnableSimpleModePick = input.EnableSimpleModePick;
                location.EnableSimpleModePack = input.EnableSimpleModePack;
                location.ValidateSource = input.ValidateSource;
                location.AddressID = input.AddressID;
                location.VarianceBinID = input.VarianceBinID;
                location.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                location.UpdatedOn = DateTime.Now;
                location.Update();
                return RedirectToAction(nameof(Index));
            }
            var addresses = Address.GetAddresses(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var packageDimensions = PackageDimension.GetPackageDimensions(CompanyID);
            ViewData["AddressID"] = new SelectList(addresses, "AddressID", "FullName", input.AddressID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", CompanyID);
            ViewData["DefaultPackageDimensionID"] = new SelectList(packageDimensions, "PackageDimensionID", "Name", input.DefaultPackageDimensionID);
            return View("~/Views/Inventory/Location/Edit.cshtml", input);
        }

        [Authorize(Policy = "Inventory-Location-Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            var location = new Location(CompanyID, id);
            if (location == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(location);
            return View("~/Views/Inventory/Location/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var location = new Location(CompanyID, id);
            if (location != null)
                location.Delete();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Insert([FromBody] CRUDModel value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel value)
        {
            return Json(value.Value ?? new LocationViewModel());
        }


        public IActionResult Remove([FromBody] CRUDModel<LocationViewModel> value)
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
            IEnumerable<LocationViewModel> dataSource = new List<LocationViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    LocationFilter locationFilter = new();
                    dataSource = Location.GetLocations(
                        CompanyID,
                        locationFilter,
                        dm.Take,
                        dm.Skip / dm.Take + 1,
                        out totalRecord
                    ).Select(EntityConvertViewModel);
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
