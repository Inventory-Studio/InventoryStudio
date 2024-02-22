using InventoryStudio.File;
using InventoryStudio.Models.OrderManagement.SalesOrder;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class SalesOrderController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public SalesOrderController(IHttpContextAccessor httpContextAccessor)
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
            var salesOrders = SalesOrder.GetSalesOrders(CompanyID);
            var list = new List<SalesOrderViewModel>();
            foreach (var salesOrder in salesOrders)
            {
                list.Add(EntityConvertViewModel(salesOrder));
            }
            return View("~/Views/OrderManagement/SalesOrder/Index.cshtml", list);
        }

        private SalesOrderViewModel EntityConvertViewModel(SalesOrder salesOrder)
        {
            var viewModel = new SalesOrderViewModel();
            viewModel.SalesOrderID = salesOrder.SalesOrderID;
            if (!string.IsNullOrEmpty(salesOrder.CompanyID))
            {
                var company = new Company(salesOrder.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }
            if (!string.IsNullOrEmpty(salesOrder.CustomerID))
            {
                var customer = new Customer(CompanyID, salesOrder.CustomerID);
                if (customer != null)
                    viewModel.Customer = customer.EmailAddress;
            }
            viewModel.PONumber = salesOrder.PONumber;
            viewModel.TranDate = salesOrder.TranDate;
            if (!string.IsNullOrEmpty(salesOrder.LocationID))
            {
                var location = new ISLibrary.OrderManagement.Location(CompanyID, salesOrder.LocationID);
                if (location != null)
                    viewModel.Location = location.LocationName;
            }
            if (!string.IsNullOrEmpty(salesOrder.BillToAddressID))
            {
                var address = new ISLibrary.OrderManagement.Address(salesOrder.BillToAddressID);
                if (address != null)
                    viewModel.BillToAddress = address.FullName;
            }
            if (!string.IsNullOrEmpty(salesOrder.ShipToAddressID))
            {
                var address = new ISLibrary.OrderManagement.Address(salesOrder.ShipToAddressID);
                if (address != null)
                    viewModel.ShipToAddress = address.FullName;
            }

            viewModel.ShippingAmount = salesOrder.ShippingAmount;
            viewModel.ShippingTaxAmount = salesOrder.ShippingTaxAmount;
            viewModel.ItemTaxAmount = salesOrder.ItemTaxAmount;
            viewModel.DiscountAmount = salesOrder.DiscountAmount;
            viewModel.SalesSource = salesOrder.SalesSource;
            viewModel.ShippingMethod = salesOrder.ShippingMethod;
            viewModel.ShippingCarrier = salesOrder.ShippingCarrier;
            viewModel.ShippingPackage = salesOrder.ShippingPackage;
            viewModel.ShippingServiceCode = salesOrder.ShippingServiceCode;
            viewModel.ShipFrom = salesOrder.ShipFrom;
            viewModel.ShipTo = salesOrder.ShipTo;
            viewModel.Status = salesOrder.Status;
            viewModel.IsClosed = salesOrder.IsClosed;
            viewModel.ExternalID = salesOrder.ExternalID;
            viewModel.InternalNote = salesOrder.InternalNote;
            viewModel.CustomerNote = salesOrder.CustomerNote;
            viewModel.GiftMessage = salesOrder.GiftMessage;
            viewModel.LabelMessage = salesOrder.LabelMessage;
            viewModel.ReferenceNumber = salesOrder.ReferenceNumber;
            viewModel.SignatureRequired = salesOrder.SignatureRequired;
            viewModel.ShopifyOrderID = salesOrder.ShopifyOrderID;
            if (!string.IsNullOrEmpty(salesOrder.CreatedBy))
            {
                var user = new AspNetUsers(salesOrder.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }
            viewModel.CreatedOn = salesOrder.CreatedOn;
            if (!string.IsNullOrEmpty(salesOrder.UpdatedBy))
            {
                var user = new AspNetUsers(salesOrder.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }
            viewModel.UpdatedOn = salesOrder.UpdatedOn;
            return viewModel;
        }


        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var salesOrder = new SalesOrder(CompanyID, id);
            if (salesOrder == null)
                return NotFound();
            var detailViewModel = EntityConvertViewModel(salesOrder);
            return View("~/Views/OrderManagement/SalesOrder/Details.cshtml", detailViewModel);
        }


        public IActionResult Create()
        {
            var addresses = Address.GetAddresses(CompanyID);
            var customers = Customer.GetCustomers(CompanyID);
            var locations = ISLibrary.OrderManagement.Location.GetLocations(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["BillToAddressID"] = new SelectList(addresses, "AddressID", "FullName");
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "EmailAddress");
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["ShipToAddressID"] = new SelectList(addresses, "AddressID", "FullName");
            return View("~/Views/OrderManagement/SalesOrder/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CompanyID,CustomerID,PONumber,TranDate,LocationID,BillToAddressID,ShipToAddressID,ShippingAmount,ShippingTaxAmount,ItemTaxAmount,DiscountAmount,SalesSource,ShippingMethod,ShippingCarrier,ShippingPackage,ShippingServiceCode,ShipFrom,ShipTo,Status,IsClosed,ExternalID,InternalNote,CustomerNote,GiftMessage,LabelMessage,ReferenceNumber,SignatureRequired,ShopifyOrderID")] CreateSalesOrderViewModel input)
        {
            if (ModelState.IsValid)
            {
                var salesOrder = new SalesOrder();
                salesOrder.CompanyID = input.CompanyID;
                salesOrder.CustomerID = input.CustomerID;
                salesOrder.PONumber = input.PONumber;
                salesOrder.TranDate = input.TranDate;
                salesOrder.LocationID = input.LocationID;
                salesOrder.BillToAddressID = input.BillToAddressID;
                salesOrder.ShipToAddressID = input.ShipToAddressID;
                salesOrder.ShippingAmount = input.ShippingAmount;
                salesOrder.ShippingTaxAmount = input.ShippingTaxAmount;
                salesOrder.ItemTaxAmount = input.ItemTaxAmount;
                salesOrder.DiscountAmount = input.DiscountAmount;
                salesOrder.SalesSource = input.SalesSource;
                salesOrder.ShippingMethod = input.ShippingMethod;
                salesOrder.ShippingCarrier = input.ShippingCarrier;
                salesOrder.ShippingPackage = input.ShippingPackage;
                salesOrder.ShippingServiceCode = input.ShippingServiceCode;
                salesOrder.ShipFrom = input.ShipFrom;
                salesOrder.ShipTo = input.ShipTo;
                salesOrder.Status = input.Status;
                salesOrder.IsClosed = input.IsClosed;
                salesOrder.ExternalID = input.ExternalID;
                salesOrder.InternalNote = input.InternalNote;
                salesOrder.CustomerNote = input.CustomerNote;
                salesOrder.GiftMessage = input.GiftMessage;
                salesOrder.LabelMessage = input.LabelMessage;
                salesOrder.ReferenceNumber = input.ReferenceNumber;
                salesOrder.SignatureRequired = input.SignatureRequired;
                salesOrder.ShopifyOrderID = input.ShopifyOrderID;
                salesOrder.CreatedOn = DateTime.Now;
                salesOrder.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salesOrder.Create();
                return RedirectToAction(nameof(Index));
            }
            var addresses = Address.GetAddresses(CompanyID);
            var customers = Customer.GetCustomers(CompanyID);
            var locations = ISLibrary.OrderManagement.Location.GetLocations(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["BillToAddressID"] = new SelectList(addresses, "AddressID", "FullName", input.BillToAddressID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "EmailAddress", input.CustomerID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
            ViewData["ShipToAddressID"] = new SelectList(addresses, "AddressID", "FullName", input.ShipToAddressID);
            return View("~/Views/OrderManagement/SalesOrder/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var salesOrder = new SalesOrder(CompanyID, id);
            if (salesOrder == null)
                return NotFound();

            var addresses = Address.GetAddresses(CompanyID);
            var customers = Customer.GetCustomers(CompanyID);
            var locations = ISLibrary.OrderManagement.Location.GetLocations(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["BillToAddressID"] = new SelectList(addresses, "AddressID", "FullName", salesOrder.BillToAddressID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", salesOrder.CompanyID);
            ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "EmailAddress", salesOrder.CustomerID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", salesOrder.LocationID);
            ViewData["ShipToAddressID"] = new SelectList(addresses, "AddressID", "FullName", salesOrder.ShipToAddressID);
            var viewModel = new EditSalesOrderViewModel();
            viewModel.SalesOrderID = salesOrder.SalesOrderID;
            viewModel.CompanyID = salesOrder.CompanyID;
            viewModel.CustomerID = salesOrder.CustomerID;
            viewModel.PONumber = salesOrder.PONumber;
            viewModel.TranDate = salesOrder.TranDate;
            viewModel.LocationID = salesOrder.LocationID;
            viewModel.BillToAddressID = salesOrder.BillToAddressID;
            viewModel.ShipToAddressID = salesOrder.ShipToAddressID;
            viewModel.ShippingAmount = salesOrder.ShippingAmount;
            viewModel.ShippingTaxAmount = salesOrder.ShippingTaxAmount;
            viewModel.ItemTaxAmount = salesOrder.ItemTaxAmount;
            viewModel.DiscountAmount = salesOrder.DiscountAmount;
            viewModel.SalesSource = salesOrder.SalesSource;
            viewModel.ShippingMethod = salesOrder.ShippingMethod;
            viewModel.ShippingCarrier = salesOrder.ShippingCarrier;
            viewModel.ShippingPackage = salesOrder.ShippingPackage;
            viewModel.ShippingServiceCode = salesOrder.ShippingServiceCode;
            viewModel.ShipFrom = salesOrder.ShipFrom;
            viewModel.ShipTo = salesOrder.ShipTo;
            viewModel.Status = salesOrder.Status;
            viewModel.IsClosed = salesOrder.IsClosed;
            viewModel.ExternalID = salesOrder.ExternalID;
            viewModel.InternalNote = salesOrder.InternalNote;
            viewModel.CustomerNote = salesOrder.CustomerNote;
            viewModel.GiftMessage = salesOrder.GiftMessage;
            viewModel.LabelMessage = salesOrder.LabelMessage;
            viewModel.ReferenceNumber = salesOrder.ReferenceNumber;
            viewModel.SignatureRequired = salesOrder.SignatureRequired;
            viewModel.ShopifyOrderID = salesOrder.ShopifyOrderID;
            return View("~/Views/OrderManagement/SalesOrder/Edit.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("SalesOrderID,CompanyID,CustomerID,PONumber,TranDate,LocationID,BillToAddressID,ShipToAddressID,ShippingAmount,ShippingTaxAmount,ItemTaxAmount,DiscountAmount,SalesSource,ShippingMethod,ShippingCarrier,ShippingPackage,ShippingServiceCode,ShipFrom,ShipTo,Status,IsClosed,ExternalID,InternalNote,CustomerNote,GiftMessage,LabelMessage,ReferenceNumber,SignatureRequired,ShopifyOrderID")] EditSalesOrderViewModel input)
        {
            if (id != input.SalesOrderID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var salesOrder = new SalesOrder(CompanyID, input.SalesOrderID);
                if (salesOrder == null)
                    return NotFound();
                salesOrder.CompanyID = input.CompanyID;
                salesOrder.CustomerID = input.CustomerID;
                salesOrder.PONumber = input.PONumber;
                salesOrder.TranDate = input.TranDate;
                salesOrder.LocationID = input.LocationID;
                salesOrder.BillToAddressID = input.BillToAddressID;
                salesOrder.ShipToAddressID = input.ShipToAddressID;
                salesOrder.ShippingAmount = input.ShippingAmount;
                salesOrder.ShippingTaxAmount = input.ShippingTaxAmount;
                salesOrder.ItemTaxAmount = input.ItemTaxAmount;
                salesOrder.DiscountAmount = input.DiscountAmount;
                salesOrder.SalesSource = input.SalesSource;
                salesOrder.ShippingMethod = input.ShippingMethod;
                salesOrder.ShippingCarrier = input.ShippingCarrier;
                salesOrder.ShippingPackage = input.ShippingPackage;
                salesOrder.ShippingServiceCode = input.ShippingServiceCode;
                salesOrder.ShipFrom = input.ShipFrom;
                salesOrder.ShipTo = input.ShipTo;
                salesOrder.Status = input.Status;
                salesOrder.IsClosed = input.IsClosed;
                salesOrder.ExternalID = input.ExternalID;
                salesOrder.InternalNote = input.InternalNote;
                salesOrder.CustomerNote = input.CustomerNote;
                salesOrder.GiftMessage = input.GiftMessage;
                salesOrder.LabelMessage = input.LabelMessage;
                salesOrder.ReferenceNumber = input.ReferenceNumber;
                salesOrder.SignatureRequired = input.SignatureRequired;
                salesOrder.ShopifyOrderID = input.ShopifyOrderID;
                salesOrder.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salesOrder.Update();
                return RedirectToAction(nameof(Index));
            }
            var addresses = Address.GetAddresses(CompanyID);
            var customers = Customer.GetCustomers(CompanyID);
            var locations = ISLibrary.OrderManagement.Location.GetLocations(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["BillToAddressID"] = new SelectList(addresses, "AddressID", "FullName", input.BillToAddressID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "EmailAddress", input.CustomerID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
            ViewData["ShipToAddressID"] = new SelectList(addresses, "AddressID", "FullName", input.ShipToAddressID);
            return View("~/Views/OrderManagement/SalesOrder/Edit.cshtml", input);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            var salesOrder = new SalesOrder(CompanyID, id);
            if (salesOrder == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(salesOrder);
            return View("~/Views/OrderManagement/SalesOrder/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var salesOrder = new SalesOrder(CompanyID, id);
            if (salesOrder != null)
                salesOrder.Delete();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost("import")]
        public async Task<IActionResult> import(IFormFile file)
        {
            try
            {
                var fileType = Path.GetExtension(file.FileName);
                var _fileHandler = FileHandlerFactory.CreateFileHandler<SalesOrder>(fileType);
                var records = await _fileHandler.Import(file);
                //【ToDo】Process import logic
                return Ok(records);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(string fileType, string id)
        {
            try
            {
                var salesOrder = new SalesOrder(CompanyID, id);
                var _fileHandler = FileHandlerFactory.CreateFileHandler<SalesOrder>(fileType);
                var fileBytes = await _fileHandler.Export(new[] { salesOrder });
                if (fileType.Contains("text/csv"))
                {
                    return File(fileBytes, "text/csv", "filename.csv");
                }
                else if (fileType.Contains("xlsx/xls"))
                {
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "filename.xlsx");
                }
                else
                {
                    return BadRequest("Unsupported file type.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
