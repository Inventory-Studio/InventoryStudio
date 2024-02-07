using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.PackageDimension
{
    public class CreatePackageDimensionViewModel
    {
        [DisplayName("Company ID")]
        public string CompanyID { get; set; }

        [DisplayName("Name")]
        public string? Name { get; set; }

        [DisplayName("Length")]
        public decimal? Length { get; set; }

        [DisplayName("Width")]
        public decimal? Width { get; set; }

        [DisplayName("Height")]
        public decimal? Height { get; set; }

        [DisplayName("Weight")]
        public decimal? Weight { get; set; }

        [DisplayName("Weight Unit")]
        public string? WeightUnit { get; set; }

        [DisplayName("Cost")]
        public decimal? Cost { get; set; }

        [DisplayName("Shipping Package")]
        public string? ShippingPackage { get; set; }

        [DisplayName("Template")]
        public string? Template { get; set; }
    }
}
