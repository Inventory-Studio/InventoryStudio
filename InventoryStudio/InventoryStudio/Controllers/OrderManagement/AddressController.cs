using InventoryStudio.Models.OrderManagement.Address;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using InventoryStudio.Models.AccessToken;
using ISLibrary.AspNet;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class AddressController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressController(IHttpContextAccessor httpContextAccessor)
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
            var addresses = Address.GetAddresses(CompanyID);
            var list = new List<AddressViewModel>();
            foreach (var address in addresses)
            {
                list.Add(EntitieConvertViewModel(address));
            }
            return View("~/Views/OrderManagement/Address/Index.cshtml", list);
        }

        private AddressViewModel EntitieConvertViewModel(Address address)
        {
            var viewModel = new AddressViewModel();
            viewModel.AddressID = address.AddressID;
            if (!string.IsNullOrEmpty(address.CompanyID))
            {
                var company = new Company(address.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }

            if (!string.IsNullOrEmpty(address.CountryID))
            {
                var country = new AddressCountry(address.CountryID);
                if (country != null)
                    viewModel.Country = country.CountryName;
            }

            viewModel.FullName = address.FullName;
            viewModel.Attention = address.Attention;
            viewModel.CompanyName = address.CompanyName;
            viewModel.Address1 = address.Address1;
            viewModel.Address2 = address.Address2;
            viewModel.Address3 = address.Address3;
            viewModel.City = address.City;
            viewModel.State = address.State;
            viewModel.PostalCode = address.PostalCode;
            viewModel.Email = address.Email;
            viewModel.Phone = address.Phone;
            viewModel.Zone = address.Zone;
            viewModel.IsInvalidAddress = address.IsInvalidAddress;
            viewModel.IsAddressUpdated = address.IsAddressUpdated;
            if (!string.IsNullOrEmpty(address.UpdatedBy))
            {
                var user = new AspNetUsers(address.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }

            viewModel.UpdatedOn = address.UpdatedOn;
            if (!string.IsNullOrEmpty(address.CreatedBy))
            {
                var user = new AspNetUsers(address.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }

            viewModel.CreatedOn = address.CreatedOn;

            var auditDataList = AuditData.GetAuditDatas("Address", viewModel.AddressID);
            viewModel.AuditDataList = auditDataList;
            return viewModel;
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var address = new Address(id);
            if (address == null)
                return NotFound();
            var detailViewModel = EntitieConvertViewModel(address);
            return View("~/Views/OrderManagement/Address/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName");
            return View("~/Views/OrderManagement/Address/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAddressViewModel input)
        {
            if (ModelState.IsValid)
            {
                var address = new Address();
                address.CompanyID = CompanyID;
                address.CountryID = input.CountryID;
                address.FullName = input.FullName;
                address.Attention = input.Attention;
                address.CompanyName = input.CompanyName;
                address.Address1 = input.Address1;
                address.Address2 = input.Address2;
                address.Address3 = input.Address3;
                address.City = input.City;
                address.State = input.State;
                address.PostalCode = input.PostalCode;
                address.Email = input.Email;
                address.Phone = input.Phone;
                address.Zone = input.Zone;
                address.IsInvalidAddress = input.IsInvalidAddress;
                address.IsAddressUpdated = input.IsAddressUpdated;
                address.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                address.CreatedOn = DateTime.Now;
                address.Create();
            }
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName", input.CountryID);
            return View("~/Views/OrderManagement/Address/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var address = new Address(id);
            if (address == null)
                return NotFound();
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName", address.CountryID);
            var viewModel = new EditAddressViewModel();
            viewModel.AddressID = address.AddressID;
            viewModel.CountryID = address.CountryID;
            viewModel.FullName = address.FullName;
            viewModel.Attention = address.Attention;
            viewModel.CompanyName = address.CompanyName;
            viewModel.Address1 = address.Address1;
            viewModel.Address2 = address.Address2;
            viewModel.Address3 = address.Address3;
            viewModel.City = address.City;
            viewModel.State = address.State;
            viewModel.PostalCode = address.PostalCode;
            viewModel.Email = address.Email;
            viewModel.Phone = address.Phone;
            viewModel.Zone = address.Zone;
            viewModel.IsInvalidAddress = address.IsInvalidAddress;
            viewModel.IsAddressUpdated = address.IsAddressUpdated;
            return View("~/Views/OrderManagement/Address/Edit.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, EditAddressViewModel input)
        {
            if (id != input.AddressID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var address = new Address(input.AddressID);
                address.CompanyID = CompanyID;
                address.CountryID = input.CountryID;
                address.FullName = input.FullName;
                address.Attention = input.Attention;
                address.CompanyName = input.CompanyName;
                address.Address1 = input.Address1;
                address.Address2 = input.Address2;
                address.Address3 = input.Address3;
                address.City = input.City;
                address.State = input.State;
                address.PostalCode = input.PostalCode;
                address.Email = input.Email;
                address.Phone = input.Phone;
                address.Zone = input.Zone;
                address.IsInvalidAddress = input.IsInvalidAddress;
                address.IsAddressUpdated = input.IsAddressUpdated;
                address.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                address.UpdatedOn = DateTime.Now;
                address.Update();
                return RedirectToAction(nameof(Index));
            }
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName", input.CountryID);
            return View("~/Views/OrderManagement/Address/Edit.cshtml", input);
        }

        public IActionResult Delete(string? id)
        {
            if (id == null)
                return NotFound();
            var address = new Address(id);
            if (address == null)
                return NotFound();
            var viewModel = EntitieConvertViewModel(address);
            return View("~/Views/OrderManagement/Address/Delete.cshtml", viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var address = new Address(id);
            if (address != null)
                address.Delete();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Insert([FromBody] CRUDModel<AddressViewModel> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<AddressViewModel> value)
        {
            return Json(value.Value ?? new AddressViewModel());
        }

        public IActionResult Remove([FromBody] CRUDModel<AddressViewModel> value)
        {
            if (value.Key != null)
            {
                DeleteConfirmed(value.Key.ToString() ?? "");
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AddressViewModel> dataSource = new List<AddressViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    AddressFilter addressFilter = new();
                    dataSource = Address.GetAddresses(
                        company.Value,
                        addressFilter,
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