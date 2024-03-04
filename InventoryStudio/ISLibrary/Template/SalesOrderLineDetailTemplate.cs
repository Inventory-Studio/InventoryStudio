using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class SalesOrderLineDetailTemplate
    {
        /// <summary>
        /// references Company Table CompanyID
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        [Required]
        public decimal Quantity { get; set; }

        public string? SerialLotNumber { get; set; }

        /// <summary>
        ///  references Bin Table BinID
        /// </summary>
        public string? Bin { get; set; }

        /// <summary>
        /// references Inventory Table InventoryID
        /// </summary>
        [Required]
        public string Inventory { get; set; } = null!;
    }
}
