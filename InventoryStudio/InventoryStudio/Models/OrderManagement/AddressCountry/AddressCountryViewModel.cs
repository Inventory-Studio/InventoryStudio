using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class AddressCountryViewModel
    {
        public string CountryID { get; set; } = null!;

      
        public string CountryName { get; set; } = null!;

        public string USPSCountryName { get; set; } = null!;

        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EelPfc { get; set; }
    }
}
