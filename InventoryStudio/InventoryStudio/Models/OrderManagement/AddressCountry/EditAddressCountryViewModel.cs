using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.AddressCountry
{
    public class EditAddressCountryViewModel
    {
        public string CountryId { get; set; } = null!;

        [Required]
        public string CountryName { get; set; } = null!;

        [Required]
        public string UspscountryName { get; set; } = null!;

        public bool IsEligibleForPltfedEx { get; set; }

        public string? EelPfc { get; set; }
    }
}
