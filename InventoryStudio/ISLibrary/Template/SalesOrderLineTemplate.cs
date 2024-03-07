﻿using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class SalesOrderLineTemplate
    {
        /// <summary>
        /// references Company Table CompanyID
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        /// <summary>
        ///  references Location Table LocationID
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        ///  references Item Table ItemID
        /// </summary>
        public string? Item { get; set; }

        /// <summary>
        ///references  SalesOrderLine Table ParentSalesOrderLineID
        /// </summary>
        public string? ParentSalesOrderLine { get; set; }

        public string? ItemSKU { get; set; } = null!;

        public string? ItemName { get; set; }

        public string? ItemImageURL { get; set; }

        public string? ItemUPC { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public decimal? QuantityCommitted { get; set; }

        public decimal? QuantityShipped { get; set; }

        /// <summary>
        /// references ItemUnit Table ItemUnitID
        /// </summary>
        public string? ItemUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TaxRate { get; set; }

        public string? Status { get; set; }

        public string? ExternalID { get; set; }

        public List<SalesOrderLineDetailTemplate> SalesOrderLineDetails { get; set; } = new List<SalesOrderLineDetailTemplate>();
    }
}