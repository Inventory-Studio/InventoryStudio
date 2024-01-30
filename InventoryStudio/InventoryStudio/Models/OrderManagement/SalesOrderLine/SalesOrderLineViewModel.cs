namespace InventoryStudio.Models.OrderManagement.SalesOrderLine
{
    public class SalesOrderLineViewModel
    {
        public long SalesOrderLineId { get; set; }

        public string Company { get; set; }

        public string SalesOrder { get; set; }

        public string? Location { get; set; }

        public string? Item { get; set; }

        public long? ParentSalesOrderLineId { get; set; }

        public string? ItemSku { get; set; }

        public string? ItemName { get; set; }

        public string? ItemImageUrl { get; set; }

        public string? ItemUpc { get; set; }

        public string? Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal? QuantityCommitted { get; set; }

        public decimal? QuantityShipped { get; set; }

        public long? ItemUnitId { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TaxRate { get; set; }

        public string? Status { get; set; }

        public string? ExternalId { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
