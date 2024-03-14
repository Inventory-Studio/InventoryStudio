using InventoryStudio.Models.OrderManagement.SalesOrderLineDetail;

namespace InventoryStudio.Models.OrderManagement.SalesOrderLine
{
    public class CreateSalesOrderLineViewModel
    {

        public string? LocationID { get; set; }

        public string? ItemID { get; set; }

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

        public List<CreateSalesOrderLineDetailViewModel> SalesOrderLineDetails { get; set; } = new List<CreateSalesOrderLineDetailViewModel>();

    }
}
