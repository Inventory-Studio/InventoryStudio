namespace InventoryStudio.Models.OrderManagement.SalesOrderLine
{
    public class SalesOrderLineViewModel
    {
        public string SalesOrderLineID { get; set; }

        public string Company { get; set; }

        public string SalesOrder { get; set; }

        public string? Location { get; set; }

        public string? Item { get; set; }

        public string? ParentSalesOrderLineID { get; set; }

        public string? ItemSKU { get; set; }

        public string? ItemName { get; set; }

        public string? ItemImageURL { get; set; }

        public string? ItemUPC { get; set; }

        public string? Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal? QuantityCommitted { get; set; }

        public decimal? QuantityShipped { get; set; }

        public string? ItemUnitID { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TaxRate { get; set; }

        public string? Status { get; set; }

        public string? ExternalID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
