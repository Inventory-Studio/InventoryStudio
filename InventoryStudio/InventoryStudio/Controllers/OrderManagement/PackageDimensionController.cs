using InventoryStudio.Models.OrderManagement.PackageDimension;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class PackageDimensionController : Controller
    {
        public IActionResult Index()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var packageDimensions = PackageDimension.GetPackageDimensions(company.Value);
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
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var packageDimension = new PackageDimension(company.Value, id);
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
        public IActionResult Create([Bind("CompanyID,Name,Length,Width,Height,Weight,WeightUnit,Cost,ShippingPackage,Template")] CreatePackageDimensionViewModel input)
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
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var packageDimension = new PackageDimension(company.Value, id);
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
        public IActionResult Edit(string id, [Bind("PackageDimensionID,CompanyID,Name,Length,Width,Height,Weight,WeightUnit,Cost,ShippingPackage,Template")] EditPackageDimensionViewModel input)
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
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var packageDimension = new PackageDimension(company.Value, id);
            var viewModel = EntitieConvertViewModel(packageDimension);
            return View("~/Views/OrderManagement/PackageDimension/Delete.cshtml", viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var packageDimension = new PackageDimension(company.Value, id);
            if (packageDimension != null)
                packageDimension.Delete();
            return RedirectToAction(nameof(Index));
        }

    }
}
