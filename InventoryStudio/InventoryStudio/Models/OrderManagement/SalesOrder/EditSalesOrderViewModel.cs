namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class EditSalesOrderViewModel
    {
        public string SalesOrderID { get; set; }

        public string CompanyID { get; set; }

        public string? CustomerID { get; set; }

        public string PONumber { get; set; } = null!;

        public DateTime TranDate { get; set; }

        public string? LocationID { get; set; }

        public string? BillToAddressID { get; set; }

        public string ShipToAddressID { get; set; }

        public decimal? ShippingAmount { get; set; }

        public decimal? ShippingTaxAmount { get; set; }

        public decimal? ItemTaxAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string? SalesSource { get; set; }

        public string? ShippingMethod { get; set; }

        public string? ShippingCarrier { get; set; }

        public string? ShippingPackage { get; set; }

        public string? ShippingServiceCode { get; set; }

        public DateTime? ShipFrom { get; set; }

        public DateTime? ShipTo { get; set; }

        public string? Status { get; set; }

        public bool IsClosed { get; set; }

        public string? ExternalID { get; set; }

        public string? InternalNote { get; set; }

        public string? CustomerNote { get; set; }

        public string? GiftMessage { get; set; }

        public string? LabelMessage { get; set; }

        public string? ReferenceNumber { get; set; }

        public bool SignatureRequired { get; set; }

        public string? ShopifyOrderID { get; set; }


    }
}
