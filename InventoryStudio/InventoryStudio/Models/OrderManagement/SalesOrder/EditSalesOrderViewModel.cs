namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class EditSalesOrderViewModel
    {
        public long SalesOrderId { get; set; }

        public long CompanyId { get; set; }

        public long? CustomerId { get; set; }

        public string Ponumber { get; set; } = null!;

        public DateOnly TranDate { get; set; }

        public long? LocationId { get; set; }

        public long? BillToAddressId { get; set; }

        public long ShipToAddressId { get; set; }

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

        public string? ExternalId { get; set; }

        public string? InternalNote { get; set; }

        public string? CustomerNote { get; set; }

        public string? GiftMessage { get; set; }

        public string? LabelMessage { get; set; }

        public string? ReferenceNumber { get; set; }

        public bool SignatureRequired { get; set; }

        public string? ShopifyOrderId { get; set; }

       
    }
}
