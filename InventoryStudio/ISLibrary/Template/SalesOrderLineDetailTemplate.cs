using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class SalesOrderLineDetailTemplate
    {
        [Required]
        public string SalesOrderLineIndex { get; set; } = null!;

        /// <summary>
        /// references Company Table CompanyID
        /// filter using CompanyName
        /// </summary>
        //[Required]
        //public string Company { get; set; } = null!;

        [Required]
        public decimal Quantity { get; set; }

        /// <summary>
        ///  references Bin Table BinID
        ///  filter using BinNumber
        /// </summary>
        public string? Bin { get; set; }

        public string? InventoryNumber { get; set; }

        /// <summary>
        /// references InventoryDetail Table InventoryID
        /// filter using InventoryNumber
        /// </summary>
        [Required]
        public string InventoryDetail { get; set; } = null!;
    }
}
