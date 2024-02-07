using InventoryStudio.Models.OrderManagement.AddressCountry;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class AddressCountryController : Controller
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
        public IActionResult Create([Bind("CountryID,CountryName,USPSCountryName,IsEligibleForPLTFedEX,EEL_PFC")] CreateAddressCountryViewModel input)
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
        public IActionResult Edit(string id, [Bind("CountryID,CountryName,USPSCountryName,IsEligibleForPLTFedEX,EEL_PFC")] EditAddressCountryViewModel input)
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

    }
}
