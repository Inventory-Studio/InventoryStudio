using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class SalesOrderViewModel
    {
        [DisplayName("Sales Order ID")]
        public string SalesOrderID { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Customer")]
        public string? Customer { get; set; }

        [DisplayName("PO Number")]
        public string PONumber { get; set; } = null!;

        [DisplayName("Transaction Date")]
        public DateTime TranDate { get; set; }

        [DisplayName("Location")] 
        public string? Location { get; set; }

        [DisplayName("Bill To Address")]
        public string? BillToAddress { get; set; }

        [DisplayName("Ship To Address")]
        public string ShipToAddress { get; set; }

        [DisplayName("Shipping Amount")]
        public decimal? ShippingAmount { get; set; }

        [DisplayName("Shipping Tax Amount")]
        public decimal? ShippingTaxAmount { get; set; }

        [DisplayName("Item Tax Amount")]
        public decimal? ItemTaxAmount { get; set; }

        [DisplayName("Discount Amount")]
        public decimal? DiscountAmount { get; set; }

        [DisplayName("Sales Source")]
        public string? SalesSource { get; set; }

        [DisplayName("Shipping Method")]
        public string? ShippingMethod { get; set; }

        [DisplayName("Shipping Carrier")]
        public string? ShippingCarrier { get; set; }

        [DisplayName("Shipping Package")]
        public string? ShippingPackage { get; set; }

        [DisplayName("Shipping Service Code")]
        public string? ShippingServiceCode { get; set; }

        [DisplayName("Shipping From")]
        public DateTime? ShipFrom { get; set; }

        [DisplayName("Shipping To")]
        public DateTime? ShipTo { get; set; }

        [DisplayName("Status")]
        public string? Status { get; set; }

        [DisplayName("Is Closed")]
        public bool IsClosed { get; set; }

        [DisplayName("External ID")]
        public string? ExternalID { get; set; }

        [DisplayName("Internal Note")]
        public string? InternalNote { get; set; }

        [DisplayName("Customer Note")]
        public string? CustomerNote { get; set; }

        [DisplayName("Gift Message")]
        public string? GiftMessage { get; set; }

        [DisplayName("Label Message")]
        public string? LabelMessage { get; set; }

        [DisplayName("Reference Number")]
        public string? ReferenceNumber { get; set; }

        [DisplayName("Signature Required")]
        public bool SignatureRequired { get; set; }

        [DisplayName("Shopify Order ID")]
        public string? ShopifyOrderID { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
    }
}