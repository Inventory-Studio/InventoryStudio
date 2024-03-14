namespace InventoryStudio.Models.OrderManagement.SalesOrderLineDetail
{
    public class EditSalesOrderLineDetailViewModel
    {
        public string SalesOrderLineDetailID { get; set; }

        public string SalesOrderLineID { get; set; }

        public string? BinID { get; set; }

        public decimal Quantity { get; set; }

        public string? SerialLotNumber { get; set; }

        public string InventoryID { get; set; }

    }
}
