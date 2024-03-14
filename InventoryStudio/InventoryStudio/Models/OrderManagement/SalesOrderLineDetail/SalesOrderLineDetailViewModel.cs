namespace InventoryStudio.Models.OrderManagement.SalesOrderLineDetail
{
    public class SalesOrderLineDetailViewModel
    {
        public string SalesOrderLineDetailID { get; set; }

        public string Company { get; set; }

        public string SalesOrderLine { get; set; }

        public string? Bin { get; set; }

        public decimal Quantity { get; set; }

        public string? SerialLotNumber { get; set; }

        public string Inventory { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
