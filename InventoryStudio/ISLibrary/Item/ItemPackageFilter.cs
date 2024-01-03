using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemPackageFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemVariationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShippingCarrierID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShippingPackageID { get; set; }
        public Database.Filter.NumericSearch.SearchFilter MaxQuantity { get; set; }
    }
}
