using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class AddressCountryViewModel
    {
        public string CountryId { get; set; } = null!;

      
        public string CountryName { get; set; } = null!;

        public string UspscountryName { get; set; } = null!;

        public bool IsEligibleForPltfedEx { get; set; }

        public string? EelPfc { get; set; }
    }
}
