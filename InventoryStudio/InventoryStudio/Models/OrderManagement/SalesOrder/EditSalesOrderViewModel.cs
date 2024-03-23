using InventoryStudio.Models.OrderManagement.Address;
using InventoryStudio.Models.OrderManagement.Customer;
using InventoryStudio.Models.OrderManagement.SalesOrderLine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.SalesOrder
{
    public class EditSalesOrderViewModel
    {
        [DisplayName("Sales Order ID")]
        [Required]
        public string SalesOrderID { get; set; } = null!;


        //[Required]
        //[DisplayName("Customer ID")]
        //public string? CustomerID { get; set; }

        [Required]
        [DisplayName("Customer")]
        public EditCustomerViewModel Customer { get; set; } = null!;

        [Required(ErrorMessage = "PONumber is required")]
        [DisplayName("PO Number")]
        public string PONumber { get; set; } = null!;

        [Required]
        [DisplayName("Transaction Date")]
        public DateTime TranDate { get; set; }

        [DisplayName("Location ID")]
        public string? LocationID { get; set; }

        //[DisplayName("Bill To Address ID")]
        //public string? BillToAddressID { get; set; }

        [DisplayName("Bill To Address")]
        public EditAddressViewModel? BillToAddress { get; set; }

        //[Required]
        //[DisplayName("Ship To Addresss ID")]
        //public string ShipToAddressID { get; set; } = null!;

        [Required]
        [DisplayName("Ship To Address")]
        public EditAddressViewModel ShipToAddress { get; set; } = null!;


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

        public List<EditSalesOrderLineViewModel> SalesOrderLines { get; set; } = new List<EditSalesOrderLineViewModel>();
    }
}
