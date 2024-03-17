using InventoryStudio.Models.OrderManagement.SalesOrderLineDetail;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.SalesOrderLine
{
    public class EditSalesOrderLineViewModel
    {
        public string? SalesOrderLineID { get; set; } 

        [Required]
        public string SalesOrderID { get; set; } = null!;

        public string? LocationID { get; set; }

        public string? ItemID { get; set; }

        public string? ParentSalesOrderLineID { get; set; }

        public string? ItemSKU { get; set; }

        public string? ItemName { get; set; }

        public string? ItemImageURL { get; set; }

        public string? ItemUPC { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number")]
        public decimal Quantity { get; set; }

        public decimal? QuantityCommitted { get; set; }

        public decimal? QuantityShipped { get; set; }

        public string? ItemUnitID { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TaxRate { get; set; }

        public string? Status { get; set; }

        public string? ExternalID { get; set; }

        public List<EditSalesOrderLineDetailViewModel> SalesOrderLineDetails { get; set; } = new List<EditSalesOrderLineDetailViewModel>();

    }
}
