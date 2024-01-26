using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class AddressCountry
    {
        public string CountryID { get; set; }

        public string CountryName { get; set; }

        public string? USPSCountryName { get; set; }

        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EEL_PFC { get; set; }
    }
}
