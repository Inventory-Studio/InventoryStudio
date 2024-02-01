using InventoryStudio.Models.OrderManagement.SalesOrder;
using InventoryStudio.Models.OrderManagement.SalesOrderLine;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class SalesOrderLineController : Controller
    {
        private readonly string CompanyID = string.Empty;

        public SalesOrderLineController()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
                CompanyID = company.Value;
        }


        public IActionResult Index()
        {
            var salesOrderLines = SalesOrderLine.GetSalesOrderLines(CompanyID);
            var list = new List<SalesOrderLineViewModel>();
            foreach (var salesOrderLine in salesOrderLines)
            {
                list.Add(EntityConvertViewModel(salesOrderLine));
            }
            return View("~/Views/OrderManagement/SalesOrderLine/Index.cshtml", list);
        }

        private SalesOrderLineViewModel EntityConvertViewModel(SalesOrderLine salesOrderLine)
        {
            var viewModel = new SalesOrderLineViewModel();
            viewModel.SalesOrderLineID = salesOrderLine.SalesOrderLineID;
            if (!string.IsNullOrEmpty(salesOrderLine.CompanyID))
            {
                var company = new Company(salesOrderLine.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }
            if (!string.IsNullOrEmpty(salesOrderLine.SalesOrderID))
            {
                var salesOrder = new SalesOrder(CompanyID, salesOrderLine.SalesOrderID);
                if (salesOrder != null)
                    viewModel.SalesOrder = salesOrder.PONumber;
            }
            if (!string.IsNullOrEmpty(salesOrderLine.LocationID))
            {
                var location = new Location(CompanyID, salesOrderLine.LocationID);
                if (location != null)
                    viewModel.Location = location.LocationName;
            }
            if (!string.IsNullOrEmpty(salesOrderLine.ItemID))
            {
                var item = new Item(CompanyID, salesOrderLine.ItemID);
                if (item != null)
                    viewModel.Item = item.ItemName;
            }
            viewModel.ParentSalesOrderLineID = salesOrderLine.ParentSalesOrderLineID;
            viewModel.ItemSKU = salesOrderLine.ItemSKU;
            viewModel.ItemName = salesOrderLine.ItemName;
            viewModel.ItemImageURL = salesOrderLine.ItemImageURL;
            viewModel.ItemUPC = salesOrderLine.ItemUPC;
            viewModel.Description = salesOrderLine.Description;
            viewModel.Quantity = salesOrderLine.Quantity;
            viewModel.QuantityCommitted = salesOrderLine.QuantityCommitted;
            viewModel.QuantityShipped = salesOrderLine.QuantityShipped;
            viewModel.ItemUnitID = salesOrderLine.ItemUnitID;
            viewModel.UnitPrice = salesOrderLine.UnitPrice;
            viewModel.TaxRate = salesOrderLine.TaxRate;
            viewModel.Status = salesOrderLine.Status;
            viewModel.ExternalID = salesOrderLine.ExternalID;
            if (!string.IsNullOrEmpty(salesOrderLine.CreatedBy))
            {
                var user = new AspNetUsers(salesOrderLine.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }
            viewModel.CreatedOn = salesOrderLine.CreatedOn;
            if (!string.IsNullOrEmpty(salesOrderLine.UpdatedBy))
            {
                var user = new AspNetUsers(salesOrderLine.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }
            viewModel.UpdatedOn = salesOrderLine.UpdatedOn;
            return viewModel;
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var salesOrderLine = new SalesOrderLine(CompanyID, id);
            if (salesOrderLine == null)
                return NotFound();
            var detailViewModel = EntityConvertViewModel(salesOrderLine);
            return View("~/Views/OrderManagement/SalesOrderLine/Details.cshtml", detailViewModel);
        }


        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var items = Item.GetItems(CompanyID);
            var locations = Location.GetLocations(CompanyID);
            var salesOrders = SalesOrder.GetSalesOrders(CompanyID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            ViewData["ItemID"] = new SelectList(items, "ItemID", "ItemNumber");
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName");
            //ViewData["ParentSalesOrderLineId"] = new SelectList("", "SalesOrderLineId", "SalesOrderLineId");
            ViewData["SalesOrderID"] = new SelectList(salesOrders, "SalesOrderID", "PONumber");
            return View("~/Views/OrderManagement/SalesOrderLine/Create.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CompanyID,SalesOrderID,LocationID,ItemID,ParentSalesOrderLineID,ItemSKU,ItemName,ItemImageURL,ItemUPC,Description,Quantity,QuantityCommitted,QuantityShipped,ItemUnitID,UnitPrice,TaxRate,Status,ExternalID")] CreateSalesOrderLineViewModel input)
        {
            if (ModelState.IsValid)
            {
                var salesOrderLine = new SalesOrderLine();
                salesOrderLine.CompanyID = input.CompanyID;
                salesOrderLine.SalesOrderID = input.SalesOrderID;
                salesOrderLine.LocationID = input.LocationID;
                salesOrderLine.ItemID = input.ItemID;
                salesOrderLine.ParentSalesOrderLineID = input.ParentSalesOrderLineID;
                salesOrderLine.ItemSKU = input.ItemSKU;
                salesOrderLine.ItemName = input.ItemName;
                salesOrderLine.ItemImageURL = input.ItemImageURL;
                salesOrderLine.ItemUPC = input.ItemUPC;
                salesOrderLine.Description = input.Description;
                salesOrderLine.Quantity = input.Quantity;
                salesOrderLine.QuantityCommitted = input.QuantityCommitted;
                salesOrderLine.QuantityShipped = input.QuantityShipped;
                salesOrderLine.ItemUnitID = input.ItemUnitID;
                salesOrderLine.UnitPrice = input.UnitPrice;
                salesOrderLine.TaxRate = input.TaxRate;
                salesOrderLine.Status = input.Status;
                salesOrderLine.ExternalID = input.ExternalID;
                salesOrderLine.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salesOrderLine.Create();
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var items = Item.GetItems(CompanyID);
            var locations = Location.GetLocations(CompanyID);
            var salesOrders = SalesOrder.GetSalesOrders(CompanyID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            ViewData["ItemID"] = new SelectList(items, "ItemID", "ItemNumber", input.ItemID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
            //ViewData["ParentSalesOrderLineId"] = new SelectList("", "SalesOrderLineId", "SalesOrderLineId", input.ParentSalesOrderLineID);
            ViewData["SalesOrderID"] = new SelectList(salesOrders, "SalesOrderID", "PONumber", input.SalesOrderID);
            return View("~/Views/OrderManagement/SalesOrderLine/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var salesOrderLine = new SalesOrderLine(CompanyID, id);
            if (salesOrderLine == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var items = Item.GetItems(CompanyID);
            var locations = Location.GetLocations(CompanyID);
            var salesOrders = SalesOrder.GetSalesOrders(CompanyID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", salesOrderLine.CompanyID);
            ViewData["ItemID"] = new SelectList(items, "ItemID", "ItemNumber", salesOrderLine.ItemID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", salesOrderLine.LocationID);
            //ViewData["ParentSalesOrderLineId"] = new SelectList("", "SalesOrderLineId", "SalesOrderLineId", salesOrderLine.ParentSalesOrderLineId);
            ViewData["SalesOrderID"] = new SelectList(salesOrders, "SalesOrderID", "PONumber", salesOrderLine.SalesOrderID);
            var viewModel = new EditSalesOrderLineViewModel();
            viewModel.SalesOrderLineID = salesOrderLine.SalesOrderLineID;
            viewModel.CompanyID = salesOrderLine.CompanyID;
            viewModel.SalesOrderID = salesOrderLine.SalesOrderID;
            viewModel.LocationID = salesOrderLine.LocationID;
            viewModel.ItemID = salesOrderLine.ItemID;
            viewModel.ParentSalesOrderLineID = salesOrderLine.ParentSalesOrderLineID;
            viewModel.ItemSKU = salesOrderLine.ItemSKU;
            viewModel.ItemName = salesOrderLine.ItemName;
            viewModel.ItemImageURL = salesOrderLine.ItemImageURL;
            viewModel.ItemUPC = salesOrderLine.ItemUPC;
            viewModel.Description = salesOrderLine.Description;
            viewModel.Quantity = salesOrderLine.Quantity;
            viewModel.QuantityCommitted = salesOrderLine.QuantityCommitted;
            viewModel.QuantityShipped = salesOrderLine.QuantityShipped;
            viewModel.ItemUnitID = salesOrderLine.ItemUnitID;
            viewModel.UnitPrice = salesOrderLine.UnitPrice;
            viewModel.TaxRate = salesOrderLine.TaxRate;
            viewModel.Status = salesOrderLine.Status;
            viewModel.ExternalID = salesOrderLine.ExternalID;
            return View("~/Views/OrderManagement/SalesOrderLine/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("SalesOrderLineID,CompanyID,SalesOrderID,LocationID,ItemID,ParentSalesOrderLineID,ItemSKU,ItemName,ItemImageURL,ItemUPC,Description,Quantity,QuantityCommitted,QuantityShipped,ItemUnitID,UnitPrice,TaxRate,Status,ExternalID")] EditSalesOrderLineViewModel input)
        {
            if (id != input.SalesOrderLineID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var salesOrderLine = new SalesOrderLine(CompanyID, id);
                if (salesOrderLine == null)
                    return NotFound();
                salesOrderLine.CompanyID = input.CompanyID;
                salesOrderLine.SalesOrderID = input.SalesOrderID;
                salesOrderLine.LocationID = input.LocationID;
                salesOrderLine.ItemID = input.ItemID;
                salesOrderLine.ParentSalesOrderLineID = input.ParentSalesOrderLineID;
                salesOrderLine.ItemSKU = input.ItemSKU;
                salesOrderLine.ItemName = input.ItemName;
                salesOrderLine.ItemImageURL = input.ItemImageURL;
                salesOrderLine.ItemUPC = input.ItemUPC;
                salesOrderLine.Description = input.Description;
                salesOrderLine.Quantity = input.Quantity;
                salesOrderLine.QuantityCommitted = input.QuantityCommitted;
                salesOrderLine.QuantityShipped = input.QuantityShipped;
                salesOrderLine.ItemUnitID = input.ItemUnitID;
                salesOrderLine.UnitPrice = input.UnitPrice;
                salesOrderLine.TaxRate = input.TaxRate;
                salesOrderLine.Status = input.Status;
                salesOrderLine.ExternalID = input.ExternalID;
                salesOrderLine.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salesOrderLine.Update();
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var items = Item.GetItems(CompanyID);
            var locations = Location.GetLocations(CompanyID);
            var salesOrders = SalesOrder.GetSalesOrders(CompanyID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            ViewData["ItemID"] = new SelectList(items, "ItemId", "ItemNumber", input.ItemID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
            //ViewData["ParentSalesOrderLineID"] = new SelectList("", "SalesOrderLineID", "SalesOrderLineID", input.ParentSalesOrderLineID);
            ViewData["SalesOrderID"] = new SelectList(salesOrders, "SalesOrderID", "PONumber", input.SalesOrderID);
            return View("~/Views/OrderManagement/SalesOrderLine/Edit.cshtml", input);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            var salesOrderLine = new SalesOrderLine(CompanyID, id);
            if (salesOrderLine == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(salesOrderLine);
            return View("~/Views/OrderManagement/SalesOrderLine/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var salesOrderLine = new SalesOrderLine(CompanyID, id);
            if (salesOrderLine != null)
                salesOrderLine.Delete();
            return RedirectToAction(nameof(Index));
        }

    }
}
