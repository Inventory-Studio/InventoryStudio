using Microsoft.AspNetCore.Components.Web;

namespace InventoryStudio.Models.Company
{
    public class CreateCompanyViewModel
    {
        public string CompanyName { get; set; }

        public bool AutomateFulfillment { get; set; }

        public string ShippoAPIKey { get; set; }

        public bool IncludePackingSlipOnLabel { get; set; }

        public string DefaultFulfillmentMethod { get; set; }

        public string DefaultFulfillmentStrategy { get; set; }

        public string DefaultAllocationStrategy { get; set; }
    }
}
