namespace InventoryStudio.Models.OrderManagement.PackageDimension
{
    public class EditPackageDimensionViewModel
    {
        public string PackageDimensionID { get; set; }

        public string? CompanyID { get; set; }

        public string? Name { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string? WeightUnit { get; set; }

        public decimal? Cost { get; set; }

        public string? ShippingPackage { get; set; }

        public string? Template { get; set; }
    }
}
