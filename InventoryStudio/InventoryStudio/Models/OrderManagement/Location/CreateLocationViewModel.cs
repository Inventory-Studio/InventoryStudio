namespace InventoryStudio.Models.OrderManagement.Location
{
    public class CreateLocationViewModel
    {
        public long CompanyId { get; set; }

        public long? ParentLocationId { get; set; }

        public string? LocationNumber { get; set; }

        public string LocationName { get; set; } = null!;

        public bool UseBins { get; set; }

        public bool UseLotNumber { get; set; }

        public bool UseCartonNumber { get; set; }

        public bool UseVendorCartonNumber { get; set; }

        public bool UseSerialNumber { get; set; }

        public bool AllowMultiplePackagePerFulfillment { get; set; }

        public bool AllowAutoPick { get; set; }

        public bool AllowAutoPickApproval { get; set; }

        public bool AllowNegativeInventory { get; set; }

        public bool DefaultAddressValidation { get; set; }

        public bool DefaultSignatureRequirement { get; set; }

        public decimal? DefaultSignatureRequirementAmount { get; set; }

        public string? DefaultCountryOfOrigin { get; set; }

        public string? DefaultHscode { get; set; }

        public bool DefaultLowestShippingRate { get; set; }

        public int? MaximumPickScanRequirement { get; set; }

        public int? MaximumPackScanRequirement { get; set; }

        public string? DisplayWeightMode { get; set; }

        public string? FulfillmentCombineStatus { get; set; }

        public long? DefaultPackageDimensionId { get; set; }

        public bool EnableSimpleMode { get; set; }

        public bool EnableSimpleModePick { get; set; }

        public bool EnableSimpleModePack { get; set; }

        public bool ValidateSource { get; set; }

        public long? AddressId { get; set; }

        public long? VarianceBinId { get; set; }

    }
}
