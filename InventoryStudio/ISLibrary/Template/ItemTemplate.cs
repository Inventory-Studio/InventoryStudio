using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.Template
{
    public class ItemTemplate
    {
        /// <summary>
        /// references Company Table CompanyID
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        /// <summary>
        /// references Bin Table  BinID
        /// BinNumber
        /// </summary>
        [Required]
        public string Bin { get; set; } = null!;

        public string? ExternalID { get; set; }

        /// <summary>
        ///references Item Table ItemParentID
        /// </summary>
        public string? ItemParent { get; set; }

        [Required]
        public string ItemType { get; set; } = null!;

        [Required]
        public string ItemNumber { get; set; } = null!;

        public string? ItemName { get; set; }

        public string? SalesDescription { get; set; }

        public string? PurchaseDescription { get; set; }

        /// <summary>
        ///references LabelProfile Table LabelProfileID
        /// </summary>
        public string? LabelProfile { get; set; }

        public string? Barcode { get; set; }

        public bool IsBarcoded { get; set; }

        public bool IsShipReceiveIndividually { get; set; }

        public bool DisplayComponents { get; set; }

        /// <summary>
        /// references ItemUnit Table ItemUnitTypeID
        /// Name
        /// </summary>
        public string? ItemUnitType { get; set; }

        /// <summary>
        ///references ItemUnit Table  PrimarySalesUnitID
        /// </summary>
        public string? PrimarySalesUnit { get; set; }

        /// <summary>
        ///references ItemUnit Table  PrimaryPurchaseUnitID
        /// </summary>
        public string? PrimaryPurchaseUnit { get; set; }

        /// <summary>
        ///references ItemUnit Table  PrimaryStockUnitID
        /// </summary>
        public string? PrimaryStockUnit { get; set; }

        public string? UnitOfMeasure { get; set; }

        public decimal? PackageWeight { get; set; }

        public string? PackageWeightUOM { get; set; }

        public decimal? PackageLength { get; set; }

        public decimal? PackageWidth { get; set; }

        public decimal? PackageHeight { get; set; }

        public string? PackageDimensionUOM { get; set; }

        public string? ImageURL { get; set; }

        public string? Memo { get; set; }

        public bool? UseSingleBin { get; set; }

        public bool FulfillByKit { get; set; }

        public bool ReceiveByKit { get; set; }

        public string? HSCode { get; set; }

        public string? GoodDescription { get; set; }

        public string? CountryOfOrigin { get; set; }

    }
}
