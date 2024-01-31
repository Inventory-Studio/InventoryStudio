using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class EditAddressCountryViewModel
    {
        public string CountryID { get; set; } = null!;

        [Required]
        public string CountryName { get; set; } = null!;

        [Required]
        public string USPSCountryName { get; set; } = null!;

        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EEL_PFC { get; set; }
    }
}
