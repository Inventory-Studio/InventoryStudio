using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class AddressCountryViewModel
    {
        [DisplayName("Country ID")]
        public string CountryID { get; set; } = null!;
      
        [DisplayName("Country Name")]
        public string CountryName { get; set; } = null!;

        [DisplayName("USPS Country Name")]
        public string USPSCountryName { get; set; } = null!;

        [DisplayName("Is Eligible For PLT FedEx")]
        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EelPfc { get; set; }
    }
}
