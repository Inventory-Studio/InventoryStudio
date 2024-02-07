using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class EditAddressCountryViewModel
    {
        [DisplayName("Country ID")]
        public string CountryID { get; set; } = null!;

        [Required]
        [DisplayName("Country Name")]
        public string CountryName { get; set; } = null!;

        [Required]
        [DisplayName("USPS Country Name")]
        public string USPSCountryName { get; set; } = null!;

        [DisplayName("Is Eligible For PLT FedEx")]
        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EEL_PFC { get; set; }
    }
}
