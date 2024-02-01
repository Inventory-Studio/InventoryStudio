﻿using InventoryStudio.Models.OrderManagement.Address;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class AddressController : Controller
    {

        public IActionResult Index()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var addresses = Address.GetAddresses(company.Value);
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
            if (string.IsNullOrEmpty(address.CompanyID))
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
            if (string.IsNullOrEmpty(address.UpdatedBy))
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName");
            return View("~/Views/OrderManagement/Address/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CompanyID,FullName,Attention,CompanyName,Address1,Address2,Address3,City,State,PostalCode,CountryID,Email,Phone,Zone,IsInvalidAddress,IsAddressUpdated")] CreateAddressViewModel input)
        {
            if (ModelState.IsValid)
            {
                var address = new Address();
                address.CompanyID = input.CompanyID;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", address.CompanyID);
            ViewData["CountryID"] = new SelectList(addressCountries, "CountryID", "CountryName", address.CountryID);
            var viewModel = new EditAddressViewModel();
            viewModel.AddressID = address.AddressID;
            viewModel.CompanyID = address.CompanyID;
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
        public IActionResult Edit(string id, [Bind("AddressID,CompanyID,FullName,Attention,CompanyName,Address1,Address2,Address3,City,State,PostalCode,CountryID,Email,Phone,Zone,IsInvalidAddress,IsAddressUpdated")] EditAddressViewModel input)
        {
            if (id != input.AddressID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var address = new Address(input.AddressID);
                address.CompanyID = input.CompanyID;
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
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var addressCountries = AddressCountry.GetAddressCountries();
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
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


    }
}