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

                    //var customer = new Customer(CompanyID, input.CustomerID);
                    //if (customer == null)
                    //    return BadRequest($"Customer with ID {input.CustomerID} not found");
                    //salesOrder.CustomerID = input.CustomerID;

                    salesOrder.PONumber = input.PONumber;
                    salesOrder.TranDate = input.TranDate;
                    if (!string.IsNullOrEmpty(input.LocationID))
                    {
                        var location = new Location(CompanyID, input.LocationID);
                        if (location == null)
                            return BadRequest($"Location with ID {input.LocationID} not found");
                    }
                    salesOrder.LocationID = input.LocationID;

                    //if (!string.IsNullOrEmpty(input.BillToAddressID))
                    //{
                    //    var address = new Address(input.BillToAddressID);
                    //    if (address == null)
                    //        return BadRequest($"BillToAddress with ID {input.BillToAddressID} not found");
                    //}
                    //salesOrder.BillToAddressID = input.BillToAddressID;

                    //if (!string.IsNullOrEmpty(input.ShipToAddressID))
                    //{
                    //    var address = new Address(input.ShipToAddressID);
                    //    if (address == null)
                    //        return BadRequest($"ShipToAddress with ID {input.BillToAddressID} not found");
                    //}
                    //salesOrder.ShipToAddressID = input.ShipToAddressID;
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
            if (ModelState.IsValid || true)
            {
                if (string.IsNullOrEmpty(input.SalesOrderID))
                    return BadRequest("SalesOrderID cannot be null or empty");

                var salesOrder = new SalesOrder(CompanyID, input.SalesOrderID);
                if (salesOrder == null)
                    return NotFound($"SalesOrder with ID {input.SalesOrderID} not found");

                //salesOrder.CompanyID = CompanyID;
                //var customer = new Customer(CompanyID, input.CustomerID);
                //if (customer == null)
                //    return BadRequest($"Customer with ID {input.CustomerID} not found");
                //salesOrder.CustomerID = input.CustomerID;

                salesOrder.PONumber = input.PONumber;
                salesOrder.TranDate = input.TranDate;
                if (!string.IsNullOrEmpty(input.LocationID))
                {
                    var location = new ISLibrary.Location(CompanyID, input.LocationID);
                    if (location == null)
                        return BadRequest($"Location with ID {input.LocationID} not found");
                }
                salesOrder.LocationID = input.LocationID;
                //if (!string.IsNullOrEmpty(input.BillToAddressID))
                //{
                //    var address = new Address(input.BillToAddressID);
                //    if (address == null)
                //        return BadRequest($"BillToAddress with ID {input.BillToAddressID} not found");
                //}
                //salesOrder.BillToAddressID = input.BillToAddressID;

                //if (!string.IsNullOrEmpty(input.ShipToAddressID))
                //{
                //    var address = new Address(input.ShipToAddressID);
                //    if (address == null)
                //        return BadRequest($"ShipToAddress with ID {input.BillToAddressID} not found");
                //}
                //salesOrder.ShipToAddressID = input.ShipToAddressID;
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
            return Ok();
        }
    }
}
