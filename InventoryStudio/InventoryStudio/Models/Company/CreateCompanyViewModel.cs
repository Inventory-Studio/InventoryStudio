using System.ComponentModel;
using Microsoft.AspNetCore.Components.Web;

namespace InventoryStudio.Models.Company
{
    public class CreateCompanyViewModel
    {
        [DisplayName("Company Name")] public string CompanyName { get; set; }

        [DisplayName("Automate Fulfillment")] public bool AutomateFulfillment { get; set; }

        [DisplayName("Shippo API Key")] public string ShippoAPIKey { get; set; }

        [DisplayName("Include Packing Slip On Label")]
        public bool IncludePackingSlipOnLabel { get; set; }

        [DisplayName("Default Fulfillment Method")]
        public string DefaultFulfillmentMethod { get; set; }

        [DisplayName("Default Fulfillment Strategy")]
        public string DefaultFulfillmentStrategy { get; set; }

        [DisplayName("Default Allocation Strategy")]
        public string DefaultAllocationStrategy { get; set; }
    }
}