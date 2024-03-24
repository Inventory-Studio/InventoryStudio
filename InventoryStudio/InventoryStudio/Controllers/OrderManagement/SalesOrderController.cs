using InventoryStudio.Models.OrderManagement.SalesOrder;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;
using InventoryStudio.Models.OrderManagement.SalesOrderLine;
using InventoryStudio.Models.OrderManagement.SalesOrderLineDetail;
using static ISLibrary.OrderManagement.SalesOrder;
using Microsoft.AspNetCore.Identity;

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
                var location = new ISLibrary.Location(CompanyID, salesOrder.LocationID);
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
            try
            {
                salesOrder.Status = (enumOrderStatus)Enum.Parse(typeof(enumOrderStatus), viewModel.Status);
            }
            catch (ArgumentException)
            {
                BadRequest($"Invaliad Status: {viewModel.Status}");
            }
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

            var auditDataList = AuditData.GetAuditDatas("SalesOrder", viewModel.SalesOrderID);
            viewModel.AuditDataList = auditDataList;
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
            var locations = ISLibrary.Location.GetLocations(CompanyID);
            ViewData["BillToAddressID"] = new SelectList(addresses, "AddressID", "FullName");
            ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "EmailAddress");
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["ShipToAddressID"] = new SelectList(addresses, "AddressID", "FullName");
            return View("~/Views/OrderManagement/SalesOrder/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(CreateSalesOrderViewModel input)
        {
            if (ModelState.IsValid)
            {
                var salesOrder = new SalesOrder();
                salesOrder.CompanyID = CompanyID;

                salesOrder.Customer = new Customer();
                salesOrder.Customer.ClientID = input.Customer.ClientID;
                salesOrder.Customer.CompanyName = input.Customer.CompanyName;
                salesOrder.Customer.FirstName = input.Customer.FirstName;
                salesOrder.Customer.LastName = input.Customer.LastName;
                salesOrder.Customer.EmailAddress = input.Customer.EmailAddress;
                salesOrder.Customer.ExternalID = input.Customer.ExternalID;

                salesOrder.PONumber = input.PONumber;
                salesOrder.TranDate = input.TranDate;
                if (!string.IsNullOrEmpty(input.LocationID))
                {
                    var location = new ISLibrary.Location(CompanyID, input.LocationID);
                    if (location == null)
                        return BadRequest($"Location with ID {input.LocationID} not found");
                }
                salesOrder.LocationID = input.LocationID;

                salesOrder.BillToAddress = new Address();
                salesOrder.BillToAddress.FullName = input.BillToAddress.FullName;
                salesOrder.BillToAddress.Attention = input.BillToAddress.Attention;
                salesOrder.BillToAddress.CompanyName = input.BillToAddress.CompanyName;
                salesOrder.BillToAddress.Address1 = input.BillToAddress.Address1;
                salesOrder.BillToAddress.Address2 = input.BillToAddress.Address2;
                salesOrder.BillToAddress.Address3 = input.BillToAddress.Address3;
                salesOrder.BillToAddress.City = input.BillToAddress.City;
                salesOrder.BillToAddress.State = input.BillToAddress.State;
                salesOrder.BillToAddress.PostalCode = input.BillToAddress.PostalCode;
                salesOrder.BillToAddress.CountryID = input.BillToAddress.CountryID;
                salesOrder.BillToAddress.Email = input.BillToAddress.Email;
                salesOrder.BillToAddress.Phone = input.BillToAddress.Phone;
                salesOrder.BillToAddress.Zone = input.BillToAddress.Zone;
                salesOrder.BillToAddress.IsInvalidAddress = input.BillToAddress.IsInvalidAddress;
                salesOrder.BillToAddress.IsAddressUpdated = input.BillToAddress.IsAddressUpdated;

                salesOrder.ShipToAddress = new Address();
                salesOrder.ShipToAddress.FullName = input.ShipToAddress.FullName;
                salesOrder.ShipToAddress.Attention = input.ShipToAddress.Attention;
                salesOrder.ShipToAddress.CompanyName = input.ShipToAddress.CompanyName;
                salesOrder.ShipToAddress.Address1 = input.ShipToAddress.Address1;
                salesOrder.ShipToAddress.Address2 = input.ShipToAddress.Address2;
                salesOrder.ShipToAddress.Address3 = input.ShipToAddress.Address3;
                salesOrder.ShipToAddress.City = input.ShipToAddress.City;
                salesOrder.ShipToAddress.State = input.ShipToAddress.State;
                salesOrder.ShipToAddress.PostalCode = input.ShipToAddress.PostalCode;
                salesOrder.ShipToAddress.CountryID = input.ShipToAddress.CountryID;
                salesOrder.ShipToAddress.Email = input.ShipToAddress.Email;
                salesOrder.ShipToAddress.Phone = input.ShipToAddress.Phone;
                salesOrder.ShipToAddress.Zone = input.ShipToAddress.Zone;
                salesOrder.ShipToAddress.IsInvalidAddress = input.ShipToAddress.IsInvalidAddress;
                salesOrder.ShipToAddress.IsAddressUpdated = input.ShipToAddress.IsAddressUpdated;

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
                salesOrder.SalesOrderLines = new List<SalesOrderLine>();
                if (input.SalesOrderLines != null)
                {
                    foreach (var lineViewModel in input.SalesOrderLines)
                    {
                        var salesOrderLine = new SalesOrderLine();

                        if (!string.IsNullOrEmpty(lineViewModel.LocationID))
                        {
                            var location = new ISLibrary.Location(CompanyID, lineViewModel.LocationID);
                            if (location == null)
                                return BadRequest($"Location with ID {lineViewModel.LocationID} not found");
                        }

                        salesOrderLine.LocationID = lineViewModel.LocationID;

                        if (!string.IsNullOrEmpty(lineViewModel.ItemID))
                        {
                            var item = new Item(CompanyID, lineViewModel.ItemID);
                            if (item == null)
                                return BadRequest($"Item with ID {lineViewModel.ItemID} not found");
                        }
                        salesOrderLine.ItemID = lineViewModel.ItemID;
                        salesOrderLine.ParentSalesOrderLineID = lineViewModel.ParentSalesOrderLineID;
                        salesOrderLine.ItemName = lineViewModel.ItemName;
                        salesOrderLine.ItemImageURL = lineViewModel.ItemImageURL;
                        salesOrderLine.Description = lineViewModel.Description;
                        salesOrderLine.Quantity = lineViewModel.Quantity;

                        if (!string.IsNullOrEmpty(lineViewModel.ItemUnitID))
                        {
                            var itemUnit = new ItemUnit(lineViewModel.ItemUnitID);
                            if (itemUnit == null)
                                return BadRequest($"ItemUnit with ID {lineViewModel.ItemUnitID} not found");
                        }

                        salesOrderLine.ItemUnitID = lineViewModel.ItemUnitID;
                        salesOrderLine.UnitPrice = lineViewModel.UnitPrice;
                        salesOrderLine.TaxRate = lineViewModel.TaxRate;
                        salesOrderLine.Status = lineViewModel.Status;
                        salesOrderLine.ExternalID = lineViewModel.ExternalID;

                        salesOrderLine.SalesOrderLineDetails = new List<SalesOrderLineDetail>();
                        if (lineViewModel.SalesOrderLineDetails != null)
                        {
                            foreach (var detailViewModel in lineViewModel.SalesOrderLineDetails)
                            {
                                var salesOrderLineDetail = new SalesOrderLineDetail();

                                if (!string.IsNullOrEmpty(detailViewModel.BinID))
                                {
                                    var bin = new Bin(CompanyID, detailViewModel.BinID);
                                    if (bin == null)
                                        return BadRequest($"Bin with ID {detailViewModel.BinID} not found");
                                }

                                salesOrderLineDetail.BinID = detailViewModel.BinID;
                                salesOrderLineDetail.Quantity = detailViewModel.Quantity;
                                salesOrderLineDetail.InventoryNumber = detailViewModel.InventoryNumber;

                                if (!string.IsNullOrEmpty(detailViewModel.InventoryDetailID))
                                {
                                    var inventory = new InventoryDetail(detailViewModel.InventoryDetailID);
                                    if (inventory == null)
                                        return BadRequest($"Invneotry with ID {detailViewModel.InventoryDetailID} not found");
                                }

                                salesOrderLineDetail.InventoryDetailID = detailViewModel.InventoryDetailID;
                                salesOrderLine.SalesOrderLineDetails.Add(salesOrderLineDetail);
                            }
                        }
                        salesOrder.SalesOrderLines.Add(salesOrderLine);
                    }
                }

                salesOrder.Create();
                return RedirectToAction(nameof(Index));
            }
            var addresses = Address.GetAddresses(CompanyID);
            var customers = Customer.GetCustomers(CompanyID);
            var locations = ISLibrary.Location.GetLocations(CompanyID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
            return View("~/Views/OrderManagement/SalesOrder/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var salesOrder = new SalesOrder(CompanyID, id);
            if (salesOrder == null)
                return NotFound();


            var locations = ISLibrary.Location.GetLocations(CompanyID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);

            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", salesOrder.LocationID);

            var viewModel = new EditSalesOrderViewModel();
            viewModel.SalesOrderID = salesOrder.SalesOrderID;

            if (salesOrder.Customer != null)
            {
                viewModel.Customer = new Models.OrderManagement.Customer.EditCustomerViewModel();
                viewModel.Customer.CustomerID = salesOrder.Customer.CustomerID;
                viewModel.Customer.ClientID = salesOrder.Customer.ClientID;
                viewModel.Customer.CompanyName = salesOrder.Customer.CompanyName;
                viewModel.Customer.FirstName = salesOrder.Customer.FirstName;
                viewModel.Customer.LastName = salesOrder.Customer.LastName;
                viewModel.Customer.EmailAddress = salesOrder.Customer.EmailAddress;
                viewModel.Customer.ExternalID = salesOrder.Customer.ExternalID;
            }

            viewModel.PONumber = salesOrder.PONumber;
            viewModel.TranDate = salesOrder.TranDate;
            viewModel.LocationID = salesOrder.LocationID;


            if (salesOrder.ShipToAddress != null)
            {
                viewModel.ShipToAddress = new Models.OrderManagement.Address.EditAddressViewModel();
                viewModel.ShipToAddress.AddressID = salesOrder.ShipToAddress.AddressID;
                viewModel.ShipToAddress.FullName = salesOrder.ShipToAddress.FullName;
                viewModel.ShipToAddress.Attention = salesOrder.ShipToAddress.Attention;
                viewModel.ShipToAddress.CompanyName = salesOrder.ShipToAddress.CompanyName;
                viewModel.ShipToAddress.Address1 = salesOrder.ShipToAddress.Address1;
                viewModel.ShipToAddress.Address2 = salesOrder.ShipToAddress.Address2;
                viewModel.ShipToAddress.Address3 = salesOrder.ShipToAddress.Address3;
                viewModel.ShipToAddress.City = salesOrder.ShipToAddress.City;
                viewModel.ShipToAddress.State = salesOrder.ShipToAddress.State;
                viewModel.ShipToAddress.PostalCode = salesOrder.ShipToAddress.PostalCode;
                viewModel.ShipToAddress.CountryID = salesOrder.ShipToAddress.CountryID;
                viewModel.ShipToAddress.Email = salesOrder.ShipToAddress.Email;
                viewModel.ShipToAddress.Phone = salesOrder.ShipToAddress.Phone;
                viewModel.ShipToAddress.Zone = salesOrder.ShipToAddress.Zone;
                viewModel.ShipToAddress.IsInvalidAddress = salesOrder.ShipToAddress.IsInvalidAddress;
                viewModel.ShipToAddress.IsAddressUpdated = salesOrder.ShipToAddress.IsAddressUpdated;
            }


            if (salesOrder.BillToAddress != null)
            {
                viewModel.BillToAddress = new Models.OrderManagement.Address.EditAddressViewModel();
                viewModel.BillToAddress.AddressID = salesOrder.BillToAddress.AddressID;
                viewModel.BillToAddress.FullName = salesOrder.BillToAddress.FullName;
                viewModel.BillToAddress.Attention = salesOrder.BillToAddress.Attention;
                viewModel.BillToAddress.CompanyName = salesOrder.BillToAddress.CompanyName;
                viewModel.BillToAddress.Address1 = salesOrder.BillToAddress.Address1;
                viewModel.BillToAddress.Address2 = salesOrder.BillToAddress.Address2;
                viewModel.BillToAddress.Address3 = salesOrder.BillToAddress.Address3;
                viewModel.BillToAddress.City = salesOrder.BillToAddress.City;
                viewModel.BillToAddress.State = salesOrder.BillToAddress.State;
                viewModel.BillToAddress.PostalCode = salesOrder.BillToAddress.PostalCode;
                viewModel.BillToAddress.CountryID = salesOrder.BillToAddress.CountryID;
                viewModel.BillToAddress.Email = salesOrder.BillToAddress.Email;
                viewModel.BillToAddress.Phone = salesOrder.BillToAddress.Phone;
                viewModel.BillToAddress.Zone = salesOrder.BillToAddress.Zone;
                viewModel.BillToAddress.IsInvalidAddress = salesOrder.BillToAddress.IsInvalidAddress;
                viewModel.BillToAddress.IsAddressUpdated = salesOrder.BillToAddress.IsAddressUpdated;
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
            viewModel.Status = Convert.ToString(salesOrder.Status);
            viewModel.IsClosed = salesOrder.IsClosed;
            viewModel.ExternalID = salesOrder.ExternalID;
            viewModel.InternalNote = salesOrder.InternalNote;
            viewModel.CustomerNote = salesOrder.CustomerNote;
            viewModel.GiftMessage = salesOrder.GiftMessage;
            viewModel.LabelMessage = salesOrder.LabelMessage;
            viewModel.ReferenceNumber = salesOrder.ReferenceNumber;
            viewModel.SignatureRequired = salesOrder.SignatureRequired;
            viewModel.ShopifyOrderID = salesOrder.ShopifyOrderID;
            if (salesOrder.SalesOrderLines.Count > 0)
            {
                viewModel.SalesOrderLines = new List<EditSalesOrderLineViewModel>();
                foreach (var salesOrderLine in salesOrder.SalesOrderLines)
                {
                    var salesOrderLineViewModel = new EditSalesOrderLineViewModel();
                    salesOrderLineViewModel.SalesOrderLineID = salesOrderLine.SalesOrderLineID;
                    salesOrderLineViewModel.SalesOrderID = salesOrderLine.SalesOrderID;
                    salesOrderLineViewModel.LocationID = salesOrderLine.LocationID;
                    salesOrderLineViewModel.ItemID = salesOrderLine.ItemID;
                    salesOrderLineViewModel.ParentSalesOrderLineID = salesOrderLine.ParentSalesOrderLineID;
                    salesOrderLineViewModel.ItemName = salesOrderLine.ItemName;
                    salesOrderLineViewModel.ItemImageURL = salesOrderLine.ItemImageURL;
                    salesOrderLineViewModel.Description = salesOrderLine.Description;
                    salesOrderLineViewModel.Quantity = salesOrderLine.Quantity;
                    salesOrderLineViewModel.QuantityCommitted = salesOrderLine.QuantityCommitted;
                    salesOrderLineViewModel.QuantityShipped = salesOrderLine.QuantityShipped;
                    salesOrderLineViewModel.ItemUnitID = salesOrderLine.ItemUnitID;
                    salesOrderLineViewModel.UnitPrice = salesOrderLine.UnitPrice;
                    salesOrderLineViewModel.TaxRate = salesOrderLine.TaxRate;
                    salesOrderLineViewModel.Status = salesOrderLine.Status;
                    salesOrderLineViewModel.ExternalID = salesOrderLine.ExternalID;
                    if (salesOrderLine.SalesOrderLineDetails.Count > 0)
                    {
                        salesOrderLineViewModel.SalesOrderLineDetails = new List<EditSalesOrderLineDetailViewModel>();
                        foreach (var salesOrderLineDetail in salesOrderLine.SalesOrderLineDetails)
                        {
                            var salesOrderLineDetailViewModel = new EditSalesOrderLineDetailViewModel();
                            salesOrderLineDetailViewModel.SalesOrderLineDetailID = salesOrderLineDetail.SalesOrderLineDetailID;
                            salesOrderLineDetailViewModel.SalesOrderLineID = salesOrderLineDetail.SalesOrderLineID;
                            salesOrderLineDetailViewModel.BinID = salesOrderLineDetail.BinID;
                            salesOrderLineDetailViewModel.Quantity = salesOrderLineDetail.Quantity;
                            salesOrderLineDetailViewModel.InventoryNumber = salesOrderLineDetail.InventoryNumber;
                            salesOrderLineDetailViewModel.InventoryDetailID = salesOrderLineDetail.InventoryDetailID;
                            salesOrderLineViewModel.SalesOrderLineDetails.Add(salesOrderLineDetailViewModel);
                        }
                    }
                    viewModel.SalesOrderLines.Add(salesOrderLineViewModel);
                }
            }
            return View("~/Views/OrderManagement/SalesOrder/Edit.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromForm] EditSalesOrderViewModel input)
        {
            if (ModelState.IsValid || true)
            {
                if (string.IsNullOrEmpty(input.SalesOrderID))
                    return BadRequest("SalesOrderID cannot be null or empty");

                var salesOrder = new SalesOrder(CompanyID, input.SalesOrderID);
                if (salesOrder == null)
                    return NotFound($"SalesOrder with ID {input.SalesOrderID} not found");

                salesOrder.CompanyID = CompanyID;
                if (salesOrder.Customer != null && !string.IsNullOrEmpty(input.Customer.CustomerID))
                {
                    salesOrder.Customer.ClientID = input.Customer.ClientID;
                    salesOrder.Customer.CompanyName = input.Customer.CompanyName;
                    salesOrder.Customer.FirstName = input.Customer.FirstName;
                    salesOrder.Customer.LastName = input.Customer.LastName;
                    salesOrder.Customer.EmailAddress = input.Customer.EmailAddress;
                    salesOrder.Customer.ExternalID = input.Customer.ExternalID;
                }
                salesOrder.PONumber = input.PONumber;
                salesOrder.TranDate = input.TranDate;
                if (!string.IsNullOrEmpty(input.LocationID))
                {
                    var location = new ISLibrary.Location(CompanyID, input.LocationID);
                    if (location == null)
                        return BadRequest($"Location with ID {input.LocationID} not found");
                }
                salesOrder.LocationID = input.LocationID;

                if (salesOrder.BillToAddress != null && !string.IsNullOrEmpty(input.BillToAddress.AddressID))
                {
                    salesOrder.BillToAddress.FullName = input.BillToAddress.FullName;
                    salesOrder.BillToAddress.Attention = input.BillToAddress.Attention;
                    salesOrder.BillToAddress.CompanyName = input.BillToAddress.CompanyName;
                    salesOrder.BillToAddress.Address1 = input.BillToAddress.Address1;
                    salesOrder.BillToAddress.Address2 = input.BillToAddress.Address2;
                    salesOrder.BillToAddress.Address3 = input.BillToAddress.Address3;
                    salesOrder.BillToAddress.City = input.BillToAddress.City;
                    salesOrder.BillToAddress.State = input.BillToAddress.State;
                    salesOrder.BillToAddress.PostalCode = input.BillToAddress.PostalCode;
                    salesOrder.BillToAddress.CountryID = input.BillToAddress.CountryID;
                    salesOrder.BillToAddress.Email = input.BillToAddress.Email;
                    salesOrder.BillToAddress.Phone = input.BillToAddress.Phone;
                    salesOrder.BillToAddress.Zone = input.BillToAddress.Zone;
                    salesOrder.BillToAddress.IsInvalidAddress = input.BillToAddress.IsInvalidAddress;
                    salesOrder.BillToAddress.IsAddressUpdated = input.BillToAddress.IsAddressUpdated;
                }


                if (salesOrder.ShipToAddress != null && !string.IsNullOrEmpty(input.ShipToAddress.AddressID))
                {
                    salesOrder.ShipToAddress.FullName = input.ShipToAddress.FullName;
                    salesOrder.ShipToAddress.Attention = input.ShipToAddress.Attention;
                    salesOrder.ShipToAddress.CompanyName = input.ShipToAddress.CompanyName;
                    salesOrder.ShipToAddress.Address1 = input.ShipToAddress.Address1;
                    salesOrder.ShipToAddress.Address2 = input.ShipToAddress.Address2;
                    salesOrder.ShipToAddress.Address3 = input.ShipToAddress.Address3;
                    salesOrder.ShipToAddress.City = input.ShipToAddress.City;
                    salesOrder.ShipToAddress.State = input.ShipToAddress.State;
                    salesOrder.ShipToAddress.PostalCode = input.ShipToAddress.PostalCode;
                    salesOrder.ShipToAddress.CountryID = input.ShipToAddress.CountryID;
                    salesOrder.ShipToAddress.Email = input.ShipToAddress.Email;
                    salesOrder.ShipToAddress.Phone = input.ShipToAddress.Phone;
                    salesOrder.ShipToAddress.Zone = input.ShipToAddress.Zone;
                    salesOrder.ShipToAddress.IsInvalidAddress = input.ShipToAddress.IsInvalidAddress;
                    salesOrder.ShipToAddress.IsAddressUpdated = input.ShipToAddress.IsAddressUpdated;
                }


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
                try
                {
                    salesOrder.Status = (enumOrderStatus)Enum.Parse(typeof(enumOrderStatus), input.Status);
                }
                catch (Exception)
                {
                    return BadRequest($"Invaliad Status: {input.Status}");
                }
                salesOrder.IsClosed = input.IsClosed;
                salesOrder.ExternalID = input.ExternalID;
                salesOrder.InternalNote = input.InternalNote;
                salesOrder.CustomerNote = input.CustomerNote;
                salesOrder.GiftMessage = input.GiftMessage;
                salesOrder.LabelMessage = input.LabelMessage;
                salesOrder.ReferenceNumber = input.ReferenceNumber;
                salesOrder.SignatureRequired = input.SignatureRequired;
                salesOrder.ShopifyOrderID = input.ShopifyOrderID;


                if (input.SalesOrderLines != null)
                {
                    salesOrder.SalesOrderLines.Clear();
                    foreach (var lineViewModel in input.SalesOrderLines)
                    {
                        var salesOrderLine = new SalesOrderLine();
                        salesOrderLine.SalesOrderLineID = lineViewModel.SalesOrderLineID;
                        salesOrderLine.SalesOrderID = salesOrder.SalesOrderID;
                        if (!string.IsNullOrEmpty(lineViewModel.LocationID))
                        {
                            var location = new ISLibrary.Location(CompanyID, lineViewModel.LocationID);
                            if (location == null)
                                return BadRequest($"Location with ID {lineViewModel.LocationID} not found");
                        }
                        salesOrderLine.LocationID = lineViewModel.LocationID;
                        if (!string.IsNullOrEmpty(lineViewModel.ItemID))
                        {
                            var item = new Item(CompanyID, lineViewModel.ItemID);
                            if (item == null)
                                return BadRequest($"Item with ID {lineViewModel.ItemID} not found");
                        }
                        salesOrderLine.ItemID = lineViewModel.ItemID;
                        salesOrderLine.ParentSalesOrderLineID = lineViewModel.ParentSalesOrderLineID;
                        salesOrderLine.ItemName = lineViewModel.ItemName;
                        salesOrderLine.ItemImageURL = lineViewModel.ItemImageURL;
                        salesOrderLine.Description = lineViewModel.Description;
                        salesOrderLine.Quantity = lineViewModel.Quantity;
                        if (!string.IsNullOrEmpty(lineViewModel.ItemUnitID))
                        {
                            var itemUnit = new ItemUnit(lineViewModel.ItemUnitID);
                            if (itemUnit == null)
                                return BadRequest($"ItemUnit with ID {lineViewModel.ItemUnitID} not found");
                        }
                        salesOrderLine.ItemUnitID = lineViewModel.ItemUnitID;
                        salesOrderLine.UnitPrice = lineViewModel.UnitPrice;
                        salesOrderLine.TaxRate = lineViewModel.TaxRate;
                        salesOrderLine.Status = lineViewModel.Status;
                        salesOrderLine.ExternalID = lineViewModel.ExternalID;

                        if (lineViewModel.SalesOrderLineDetails != null)
                        {
                            foreach (var detailViewModel in lineViewModel.SalesOrderLineDetails)
                            {
                                var salesOrderLineDetail = new SalesOrderLineDetail();
                                salesOrderLineDetail.SalesOrderLineDetailID = detailViewModel.SalesOrderLineDetailID;
                                salesOrderLineDetail.SalesOrderLineID = salesOrderLine.SalesOrderLineID;
                                if (!string.IsNullOrEmpty(detailViewModel.BinID))
                                {
                                    var bin = new Bin(CompanyID, detailViewModel.BinID);
                                    if (bin == null)
                                        return BadRequest($"Bin with ID {detailViewModel.BinID} not found");
                                }
                                salesOrderLineDetail.BinID = detailViewModel.BinID;
                                salesOrderLineDetail.ItemID = lineViewModel.ItemID;
                                salesOrderLineDetail.CompanyID = CompanyID;
                                salesOrderLineDetail.Quantity = detailViewModel.Quantity;
                                salesOrderLineDetail.InventoryNumber = detailViewModel.InventoryNumber;
                                if (!string.IsNullOrEmpty(detailViewModel.InventoryDetailID))
                                {
                                    var inventory = new InventoryDetail(detailViewModel.InventoryDetailID);
                                    if (inventory == null)
                                        return BadRequest($"Invneotry with ID {detailViewModel.InventoryDetailID} not found");
                                }
                                salesOrderLineDetail.InventoryDetailID = detailViewModel.InventoryDetailID;
                                if (salesOrderLine.SalesOrderLineDetails == null)
                                {
                                    salesOrderLine.SalesOrderLineDetails = new List<SalesOrderLineDetail>();
                                }
                                salesOrderLine.SalesOrderLineDetails.Add(salesOrderLineDetail);
                            }
                        }

                        salesOrder.SalesOrderLines.Add(salesOrderLine);
                    }
                }

                salesOrder.Update();
                return RedirectToAction(nameof(Index));
            }

            var locations = ISLibrary.Location.GetLocations(CompanyID);
            ViewData["LocationID"] = new SelectList(locations, "LocationID", "LocationName", input.LocationID);
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



        [HttpGet("export")]
        public async Task<IActionResult> Export(string fileType, string id)
        {
            try
            {
                //【Todo】
                //var salesOrder = new SalesOrder(CompanyID, id);
                //var _fileHandler = FileHandlerFactory.CreateFileHandler<SalesOrder>(fileType);
                //var fileBytes = await _fileHandler.Export(new[] { salesOrder });
                //if (fileType.Contains("text/csv"))
                //{
                //    return File(fileBytes, "text/csv", "filename.csv");
                //}
                //else if (fileType.Contains("xlsx/xls"))
                //{
                //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "filename.xlsx");
                //}
                //else
                //{
                //    return BadRequest("Unsupported file type.");
                //}
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        public IActionResult Insert([FromBody] CRUDModel value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<SalesOrderViewModel> value)
        {
            return Json(value.Value ?? new SalesOrderViewModel());
        }


        public IActionResult Remove([FromBody] CRUDModel<SalesOrderViewModel> value)
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
            IEnumerable<SalesOrderViewModel> dataSource = new List<SalesOrderViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    SalesOrderFilter salesOrderFilter = new();
                    dataSource = SalesOrder.GetSalesOrders(
                        CompanyID,
                        salesOrderFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
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
