using InventoryStudio.Models.OrderManagement.SalesOrder;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static ISLibrary.OrderManagement.SalesOrder;

namespace InventoryStudio.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
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

        [HttpPost]
        public IActionResult PostSalesOrder([FromBody] CreateSalesOrderViewModel input)
        {
            try
            {
                if (input == null)
                {
                    return BadRequest("Input is null");
                }

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
                        var location = new Location(CompanyID, input.LocationID);
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
                    salesOrder.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    salesOrder.SalesOrderLines = new List<SalesOrderLine>();
                    if (input.SalesOrderLines != null)
                    {
                        foreach (var lineViewModel in input.SalesOrderLines)
                        {
                            var salesOrderLine = new SalesOrderLine();

                            if (!string.IsNullOrEmpty(lineViewModel.LocationID))
                            {
                                var location = new Location(CompanyID, lineViewModel.LocationID);
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
                            salesOrderLine.QuantityCommitted = lineViewModel.QuantityCommitted;
                            salesOrderLine.QuantityShipped = lineViewModel.QuantityShipped;

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
                                        var inventory = new Inventory(detailViewModel.InventoryDetailID);
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
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }

        [HttpPut]
        public IActionResult PutSalesOrder([FromBody] EditSalesOrderViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(input.SalesOrderID))
                        return BadRequest("SalesOrderID cannot be null or empty");

                    var salesOrder = new SalesOrder(CompanyID, input.SalesOrderID);
                    if (salesOrder == null)
                        return NotFound($"SalesOrder with ID {input.SalesOrderID} not found");

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


                    if (salesOrder.SalesOrderLines != null && input.SalesOrderLines != null)
                    {
                        foreach (var salesOrderLine in salesOrder.SalesOrderLines)
                        {
                            var currentInputSalesOrderLine = input.SalesOrderLines.Where(t => t.SalesOrderLineID == salesOrderLine.SalesOrderLineID).FirstOrDefault();
                            if (currentInputSalesOrderLine != null)
                            {
                                salesOrderLine.LocationID = currentInputSalesOrderLine.LocationID;
                                salesOrderLine.ItemID = currentInputSalesOrderLine.ItemID;
                                salesOrderLine.ParentSalesOrderLineID = currentInputSalesOrderLine.ParentSalesOrderLineID;
                                salesOrderLine.ItemName = currentInputSalesOrderLine.ItemName;
                                salesOrderLine.ItemImageURL = currentInputSalesOrderLine.ItemImageURL;
                                salesOrderLine.Description = currentInputSalesOrderLine.Description;
                                salesOrderLine.Quantity = currentInputSalesOrderLine.Quantity;
                                salesOrderLine.QuantityCommitted = currentInputSalesOrderLine.QuantityCommitted;
                                salesOrderLine.QuantityShipped = currentInputSalesOrderLine.QuantityShipped;
                                salesOrderLine.ItemUnitID = currentInputSalesOrderLine.ItemUnitID;
                                salesOrderLine.UnitPrice = currentInputSalesOrderLine.UnitPrice;
                                salesOrderLine.TaxRate = currentInputSalesOrderLine.TaxRate;
                                salesOrderLine.Status = currentInputSalesOrderLine.Status;
                                salesOrderLine.ExternalID = currentInputSalesOrderLine.ExternalID;
                            }

                            if (salesOrderLine.SalesOrderLineDetails != null && currentInputSalesOrderLine.SalesOrderLineDetails != null)
                            {
                                foreach (var salesOrderLineDetail in salesOrderLine.SalesOrderLineDetails)
                                {
                                    var currentInputSalesOrderLineDetail = currentInputSalesOrderLine.SalesOrderLineDetails.Where(t => t.SalesOrderLineDetailID == salesOrderLineDetail.SalesOrderLineDetailID).FirstOrDefault();
                                    if (currentInputSalesOrderLineDetail != null)
                                    {
                                        salesOrderLineDetail.BinID = currentInputSalesOrderLineDetail.BinID;
                                        salesOrderLineDetail.Quantity = currentInputSalesOrderLineDetail.Quantity;
                                        salesOrderLineDetail.InventoryNumber = currentInputSalesOrderLineDetail.InventoryNumber;
                                        salesOrderLineDetail.InventoryDetailID = currentInputSalesOrderLineDetail.InventoryDetailID;
                                    }
                                }
                                var addSalesOrderLineDetails = currentInputSalesOrderLine.SalesOrderLineDetails.Where(t => t.SalesOrderLineDetailID == string.Empty).ToList();
                                foreach (var currentInputSalesOrderLineDetail in addSalesOrderLineDetails)
                                {
                                    var salesOrderLineDetail = new SalesOrderLineDetail();
                                    salesOrderLineDetail.SalesOrderLineID = salesOrderLine.SalesOrderLineID;
                                    salesOrderLineDetail.BinID = currentInputSalesOrderLineDetail.BinID;
                                    salesOrderLineDetail.Quantity = currentInputSalesOrderLineDetail.Quantity;
                                    salesOrderLineDetail.InventoryNumber = currentInputSalesOrderLineDetail.InventoryNumber;
                                    salesOrderLineDetail.InventoryDetailID = currentInputSalesOrderLineDetail.InventoryDetailID;
                                    salesOrderLine.SalesOrderLineDetails.Add(salesOrderLineDetail);
                                }

                                var deleteSalesOrderLineDetails = currentInputSalesOrderLine.SalesOrderLineDetails.Where(t => currentInputSalesOrderLine.SalesOrderLineDetails.Any(t => t.SalesOrderLineDetailID == t.SalesOrderLineDetailID)).ToList();
                                foreach (var salesOrderLineDetail in deleteSalesOrderLineDetails)
                                {
                                    currentInputSalesOrderLine.SalesOrderLineDetails.Remove(salesOrderLineDetail);
                                }
                            }
                        }

                        var addSalesOrderLines = input.SalesOrderLines.Where(t => t.SalesOrderLineID == string.Empty).ToList();
                        if (addSalesOrderLines.Count > 0)
                        {
                            foreach (var currentInputSalesOrderLine in addSalesOrderLines)
                            {
                                var salesOrderLine = new SalesOrderLine();
                                salesOrderLine.SalesOrderID = salesOrder.SalesOrderID;
                                salesOrderLine.LocationID = currentInputSalesOrderLine.LocationID;
                                salesOrderLine.ItemID = currentInputSalesOrderLine.ItemID;
                                salesOrderLine.ParentSalesOrderLineID = currentInputSalesOrderLine.ParentSalesOrderLineID;
                                salesOrderLine.ItemName = currentInputSalesOrderLine.ItemName;
                                salesOrderLine.ItemImageURL = currentInputSalesOrderLine.ItemImageURL;
                                salesOrderLine.Description = currentInputSalesOrderLine.Description;
                                salesOrderLine.Quantity = currentInputSalesOrderLine.Quantity;
                                salesOrderLine.QuantityCommitted = currentInputSalesOrderLine.QuantityCommitted;
                                salesOrderLine.QuantityShipped = currentInputSalesOrderLine.QuantityShipped;
                                salesOrderLine.ItemUnitID = currentInputSalesOrderLine.ItemUnitID;
                                salesOrderLine.UnitPrice = currentInputSalesOrderLine.UnitPrice;
                                salesOrderLine.TaxRate = currentInputSalesOrderLine.TaxRate;
                                salesOrderLine.Status = currentInputSalesOrderLine.Status;
                                salesOrderLine.ExternalID = currentInputSalesOrderLine.ExternalID;
                                salesOrder.SalesOrderLines.Add(salesOrderLine);
                            }
                        }

                        var deleteSalesOrderLines = salesOrder.SalesOrderLines.Where(s => !input.SalesOrderLines.Any(i => i.SalesOrderLineID == s.SalesOrderLineID)).ToList();

                        foreach (var salesOrderLine in deleteSalesOrderLines)
                        {
                            salesOrder.SalesOrderLines.Remove(salesOrderLine);
                        }
                    }
                    salesOrder.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    salesOrder.Update();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
