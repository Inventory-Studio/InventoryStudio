using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class SalesOrderTemplate
    {
        /// <summary>
        /// references Company Table CompanyID
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        /// <summary>
        /// references Customer Table CustomerID
        /// </summary>
        public string? Customer { get; set; }

        [Required]
        public string PONumber { get; set; } = null!;

        [Required]
        public DateTime TranDate { get; set; }

        /// <summary>
        /// references Location Table LocationID
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// references Address Table AddressID
        /// </summary>
        public string? BillToAddress { get; set; }

        /// <summary>
        /// references Address Table AddressID
        /// </summary>
        [Required]
        public string ShipToAddress { get; set; } = null!;

        public decimal? ShippingAmount { get; set; }

        public decimal? ShippingTaxAmount { get; set; }

        public decimal? ItemTaxAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string? SalesSource { get; set; }

        public string? ShippingMethod { get; set; }

        public string? ShippingCarrier { get; set; }

        public string? ShippingPackage { get; set; }

        public string? ShippingServiceCode { get; set; }

        public DateTime? ShipFrom { get; set; }

        public DateTime? ShipTo { get; set; }

        public string? Status { get; set; }

        public bool IsClosed { get; set; }

        public string? ExternalID { get; set; }

        public string? InternalNote { get; set; }

        public string? CustomerNote { get; set; }

        public string? GiftMessage { get; set; }

        public string? LabelMessage { get; set; }

        public string? ReferenceNumber { get; set; }

        public bool SignatureRequired { get; set; }

        public string? ShopifyOrderID { get; set; }

        public List<SalesOrderLineTemplate> SalesOrderLines { get; set; } = new List<SalesOrderLineTemplate>();
    }

}
