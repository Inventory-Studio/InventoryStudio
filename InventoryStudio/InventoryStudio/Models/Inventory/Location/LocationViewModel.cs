using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace InventoryStudio.Models
{
    public class LocationViewModel
    {
        [DisplayName("Location ID")]
        public string LocationID { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Parent Location ID")]
        public string? ParentLocationID { get; set; }

        [DisplayName("Location Number")]
        public string? LocationNumber { get; set; }

        [DisplayName("Location Name")]
        public string LocationName { get; set; } = null!;

        [DisplayName("Use Bins")]
        public bool UseBins { get; set; }

        [DisplayName("Use Lot Number")]
        public bool UseLotNumber { get; set; }

        [DisplayName("Use Carton Number")]
        public bool UseCartonNumber { get; set; }

        [DisplayName("Use Vendor Carton Number")]
        public bool UseVendorCartonNumber { get; set; }

        [DisplayName("Use Serial Number")]
        public bool UseSerialNumber { get; set; }

        [DisplayName("Allow Multiple Package Per Fulfillment")]
        public bool AllowMultiplePackagePerFulfillment { get; set; }

        [DisplayName("Allow Auto Pick")]
        public bool AllowAutoPick { get; set; }

        [DisplayName("Allow Auto Pick Approval")]
        public bool AllowAutoPickApproval { get; set; }

        [DisplayName("Allow Negative Inventory")]
        public bool AllowNegativeInventory { get; set; }

        [DisplayName("Default Address Validation")]
        public bool DefaultAddressValidation { get; set; }

        [DisplayName("Default Signature Requirement")]
        public bool DefaultSignatureRequirement { get; set; }

        [DisplayName("Default Signature Requirement Amount")]
        public decimal? DefaultSignatureRequirementAmount { get; set; }

        [DisplayName("Default Country of Origin")]
        public string? DefaultCountryOfOrigin { get; set; }

        [DisplayName("Display HS Code")]
        public string? DefaultHSCode { get; set; }

        [DisplayName("Default Lowest Shipping Rate")]
        public bool DefaultLowestShippingRate { get; set; }

        [DisplayName("Maximum Pick Scan Requirement")]
        public int? MaximumPickScanRequirement { get; set; }

        [DisplayName("Maximum Pack Scan Requirement")]
        public int? MaximumPackScanRequirement { get; set; }

        [DisplayName("Display Weight Mode")]
        public string? DisplayWeightMode { get; set; }

        [DisplayName("Fulfillment Combine Status")]
        public string? FulfillmentCombineStatus { get; set; }

        [DisplayName("Default Package Dimension")]
        public string? DefaultPackageDimension { get; set; }

        [DisplayName("Enable Simple Mode")]
        public bool EnableSimpleMode { get; set; }

        [DisplayName("Enable Simple Mode Pick")]
        public bool EnableSimpleModePick { get; set; }

        [DisplayName("Enable Simple Mode Pack")]
        public bool EnableSimpleModePack { get; set; }

        [DisplayName("Validate Source")]
        public bool ValidateSource { get; set; }

        [DisplayName("Address")]
        public string? Address { get; set; }

        [DisplayName("Variance Bin ID")]
        public string? VarianceBinID { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [ValidateNever]
        public List<AuditData>? AuditDataList { get; set; }

        [ValidateNever]
        public List<Bin>? BinList { get; set; }
    }
}
