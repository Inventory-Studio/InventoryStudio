using System.Text.Json;
using InventoryStudio.Models.OrderManagement.AddressCountry;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class AddressCountryController : BaseController
    {
        public IActionResult Index()
        {
            var addressCountries = AddressCountry.GetAddressCountries();
            var list = new List<AddressCountryViewModel>();
            foreach (var addressCountry in addressCountries)
            {
                list.Add(EntitieConvertViewModel(addressCountry));
            }

            return View("~/Views/OrderManagement/AddressCountry/Index.cshtml", list);
        }

        private AddressCountryViewModel EntitieConvertViewModel(AddressCountry addressCountry)
        {
            var viewModel = new AddressCountryViewModel();
            viewModel.CountryID = addressCountry.CountryID;
            viewModel.CountryName = addressCountry.CountryName;
            viewModel.USPSCountryName = addressCountry.USPSCountryName;
            viewModel.EelPfc = addressCountry.EEL_PFC;
            viewModel.IsEligibleForPLTFedEX = addressCountry.IsEligibleForPLTFedEX;

            var auditDataList = AuditData.GetAuditDatas("AddressCountry", viewModel.CountryID);
            viewModel.AuditDataList = auditDataList;
            return viewModel;
        }


        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var addressCountry = new AddressCountry(id);
            if (addressCountry == null)
                return NotFound();
            var detailViewModel = EntitieConvertViewModel(addressCountry);
            return View("~/Views/OrderManagement/AddressCountry/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            return View("~/Views/OrderManagement/AddressCountry/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("CountryID,CountryName,USPSCountryName,IsEligibleForPLTFedEX,EEL_PFC")]
            CreateAddressCountryViewModel input)
        {
            if (ModelState.IsValid)
            {
                var addressCountry = new AddressCountry();
                addressCountry.CountryID = input.CountryID;
                addressCountry.CountryName = input.CountryName;
                addressCountry.USPSCountryName = input.USPSCountryName;
                addressCountry.IsEligibleForPLTFedEX = input.IsEligibleForPLTFedEX;
                addressCountry.EEL_PFC = input.EEL_PFC;
                addressCountry.Create();
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/OrderManagement/AddressCountry/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var addressCountry = new AddressCountry(id);
            if (addressCountry == null)
                return NotFound();
            var viewModel = new EditAddressCountryViewModel();
            viewModel.CountryID = addressCountry.CountryID;
            viewModel.CountryName = addressCountry.CountryName;
            viewModel.USPSCountryName = addressCountry.USPSCountryName;
            viewModel.EEL_PFC = addressCountry.EEL_PFC;
            viewModel.IsEligibleForPLTFedEX = addressCountry.IsEligibleForPLTFedEX;
            return View("~/Views/OrderManagement/AddressCountry/Edit.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id,
            [Bind("CountryID,CountryName,USPSCountryName,IsEligibleForPLTFedEX,EEL_PFC")]
            EditAddressCountryViewModel input)
        {
            if (id != input.CountryID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var addressCountry = new AddressCountry(input.CountryID);
                addressCountry.CountryName = input.CountryName;
                addressCountry.USPSCountryName = input.USPSCountryName;
                addressCountry.IsEligibleForPLTFedEX = input.IsEligibleForPLTFedEX;
                addressCountry.EEL_PFC = input.EEL_PFC;
                addressCountry.Update();
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/OrderManagement/AddressCountry/Edit.cshtml", input);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            var addressCountry = new AddressCountry(id);
            if (addressCountry == null)
                return NotFound();
            var viewModel = EntitieConvertViewModel(addressCountry);
            return View("~/Views/OrderManagement/AddressCountry/Delete.cshtml", viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var addressCountry = new AddressCountry(id);
            if (addressCountry != null)
                addressCountry.Delete();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Insert([FromBody] CRUDModel value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<AddressCountryViewModel> value)
        {
            return Json(value.Value ?? new AddressCountryViewModel());
        }

        public IActionResult Remove([FromBody] CRUDModel<AddressCountryViewModel> value)
        {
            if (value.Key != null)
            {
                DeleteConfirmed(value.Key.ToString() ?? "");
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AddressCountryViewModel> dataSource = new List<AddressCountryViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                AddressCountryFilter addressCountryFilter = new();
                dataSource = AddressCountry.GetAddressCountries(
                    addressCountryFilter,
                    dm.Take,
                    (dm.Skip / dm.Take) + 1,
                    out totalRecord
                ).Select(EntitieConvertViewModel);
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