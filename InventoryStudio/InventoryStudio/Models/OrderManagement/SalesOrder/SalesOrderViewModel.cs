namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class SalesOrderViewModel
    {
        public string SalesOrderID { get; set; }

        public string Company { get; set; }

        public string? Customer { get; set; }

        public string PONumber { get; set; } = null!;

        public DateTime TranDate { get; set; }

        public string? Location { get; set; }

        public string? BillToAddress { get; set; }

        public string ShipToAddress { get; set; }

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

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
