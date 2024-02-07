using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class CreateSalesOrderViewModel
    {
        [DisplayName("Company ID")]
        public string CompanyID { get; set; }

        [DisplayName("Customer ID")]
        public string? CustomerID { get; set; }

        [DisplayName("PO Number")]
        public string PONumber { get; set; } = null!;

        [DisplayName("Transaction Date")]
        public DateTime TranDate { get; set; }

        [DisplayName("Location ID")]
        public string? LocationID { get; set; }

        [DisplayName("Bill To Address ID")]
        public string? BillToAddressID { get; set; }

        [DisplayName("Ship To Addresss ID")]
        public string ShipToAddressID { get; set; }

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
    }
}
