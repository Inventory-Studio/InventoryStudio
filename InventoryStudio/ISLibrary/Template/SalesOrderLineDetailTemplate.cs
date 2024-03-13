using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class SalesOrderLineDetailTemplate
    {
        [Required]
        public string SaelsOrderIndex { get; set; } = null!;

        /// <summary>
        /// references Company Table CompanyID
        /// filter using CompanyName
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        [Required]
        public decimal Quantity { get; set; }

        public string? SerialLotNumber { get; set; }

        /// <summary>
        ///  references Bin Table BinID
        ///  filter using BinNumber
        /// </summary>
        public string? Bin { get; set; }

        /// <summary>
        /// references Inventory Table InventoryID
        /// filter using ItemId
        /// </summary>
        [Required]
        public string Inventory { get; set; } = null!;
    }
}
