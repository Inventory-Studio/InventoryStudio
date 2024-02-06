using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class AddressCountryFilter
    {
        public Database.Filter.StringSearch.SearchFilter CountryID { get; set; }

        public Database.Filter.StringSearch.SearchFilter CountryName { get; set; }

        public Database.Filter.StringSearch.SearchFilter USPSCountryName { get; set; }

        public Database.Filter.StringSearch.SearchFilter IsEligibleForPLTFedEX { get; set; }
        public Database.Filter.StringSearch.SearchFilter EEL_PFC { get; set; }
    }
}
