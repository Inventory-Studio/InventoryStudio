using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.SalesOrderLineDetail
{
    public class CreateSalesOrderLineDetailViewModel
    {
        public string? BinID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number")]
        public decimal Quantity { get; set; }

        public string? InventoryNumber { get; set; }

        [Required]
        public string InventoryDetailID { get; set; } = null!;
    }
}
