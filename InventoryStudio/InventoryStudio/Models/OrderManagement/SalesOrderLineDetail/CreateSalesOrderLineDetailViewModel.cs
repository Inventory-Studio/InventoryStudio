namespace InventoryStudio.Models.OrderManagement.SalesOrderLineDetail
{
    public class CreateSalesOrderLineDetailViewModel
    {
        public string? BinID { get; set; }

        public decimal Quantity { get; set; }

        public string? SerialLotNumber { get; set; }

        public string InventoryID { get; set; }
    }
}
