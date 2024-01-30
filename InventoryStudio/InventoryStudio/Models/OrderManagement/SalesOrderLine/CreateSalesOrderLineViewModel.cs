namespace InventoryStudio.Models.OrderManagement.SalesOrderLine
{
    public class CreateSalesOrderLineViewModel
    {

        public long CompanyId { get; set; }

        public long SalesOrderId { get; set; }

        public long? LocationId { get; set; }

        public long? ItemId { get; set; }

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

    }
}
