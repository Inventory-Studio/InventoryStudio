using InventoryStudio.Models.OrderManagement.SalesOrder;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult Create([FromBody] CreateSalesOrderViewModel input)
        {
            try
            {
                if (input == null)
                {
                    return BadRequest("Input is null");
                }

                var salesOrder = new SalesOrder
                {
                    CompanyID = CompanyID,
                    CustomerID = input.CustomerID,
                    PONumber = input.PONumber,
                    TranDate = input.TranDate,
                    LocationID = input.LocationID,
                    BillToAddressID = input.BillToAddressID,
                    ShipToAddressID = input.ShipToAddressID,
                    ShippingAmount = input.ShippingAmount,
                    ShippingTaxAmount = input.ShippingTaxAmount,
                    ItemTaxAmount = input.ItemTaxAmount,
                    DiscountAmount = input.DiscountAmount,
                    SalesSource = input.SalesSource,
                    ShippingMethod = input.ShippingMethod,
                    ShippingCarrier = input.ShippingCarrier,
                    ShippingPackage = input.ShippingPackage,
                    ShippingServiceCode = input.ShippingServiceCode,
                    ShipFrom = input.ShipFrom,
                    ShipTo = input.ShipTo,
                    Status = input.Status,
                    IsClosed = input.IsClosed,
                    ExternalID = input.ExternalID,
                    InternalNote = input.InternalNote,
                    CustomerNote = input.CustomerNote,
                    GiftMessage = input.GiftMessage,
                    LabelMessage = input.LabelMessage,
                    ReferenceNumber = input.ReferenceNumber,
                    SignatureRequired = input.SignatureRequired,
                    ShopifyOrderID = input.ShopifyOrderID,
                    CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                salesOrder.SalesOrderLines = new List<SalesOrderLine>();
                foreach (var lineViewModel in input.SalesOrderLines)
                {
                    var salesOrderLine = new SalesOrderLine
                    {
                        LocationID = lineViewModel.LocationID,
                        ItemID = lineViewModel.ItemID,
                        ParentSalesOrderLineID = lineViewModel.ParentSalesOrderLineID,
                        ItemSKU = lineViewModel.ItemSKU,
                        ItemName = lineViewModel.ItemName,
                        ItemImageURL = lineViewModel.ItemImageURL,
                        ItemUPC = lineViewModel.ItemUPC,
                        Description = lineViewModel.Description,
                        Quantity = lineViewModel.Quantity,
                        QuantityCommitted = lineViewModel.QuantityCommitted,
                        QuantityShipped = lineViewModel.QuantityShipped,
                        ItemUnitID = lineViewModel.ItemUnitID,
                        UnitPrice = lineViewModel.UnitPrice,
                        TaxRate = lineViewModel.TaxRate,
                        Status = lineViewModel.Status,
                        ExternalID = lineViewModel.ExternalID,

                    };
                    salesOrderLine.SalesOrderLineDetails = new List<SalesOrderLineDetail>();
                    foreach (var detailViewModel in lineViewModel.SalesOrderLineDetails)
                    {
                        var salesOrderLineDetail = new SalesOrderLineDetail
                        {
                            BinID = detailViewModel.BinID,
                            Quantity = detailViewModel.Quantity,
                            SerialLotNumber = detailViewModel.SerialLotNumber,
                            InventoryID = detailViewModel.InventoryID,
                        };

                        salesOrderLine.SalesOrderLineDetails.Add(salesOrderLineDetail);
                    }

                    salesOrder.SalesOrderLines.Add(salesOrderLine);
                }

                salesOrder.Create();

                return Ok(salesOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }
    }
}
